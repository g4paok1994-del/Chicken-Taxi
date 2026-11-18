"use strict";

(() => {
  const canvas = document.getElementById("game");
  const ctx = canvas.getContext("2d", { alpha: false });
  const $ = id => document.getElementById(id);
  const refs = {
    hud: $("hud"), overlay: $("overlay"), menu: $("menuScreen"), pauseMenu: $("pauseScreen"),
    coins: $("coins"), score: $("score"), missionTitle: $("missionTitle"),
    missionSubtitle: $("missionSubtitle"), missionTimer: $("missionTimer"),
    direction: $("direction"), distance: $("distance"), toast: $("toast"),
    speedLabel: $("speedLabel"), upgrade: $("upgrade")
  };

  const ROAD_SPACING = 190;
  const ROAD_WIDTH = 58;
  const SAVE_KEY = "chickenTaxiAndroidPrototype_v1";
  let w = 375, h = 750, pixelRatio = 1, lastFrame = 0, hudClock = 0, toastTime = 0;
  let soundContext = null;
  let saved = { coins: 0, upgrade: 0, best: 0 };

  try {
    const raw = JSON.parse(localStorage.getItem(SAVE_KEY) || "{}");
    saved.coins = Math.max(0, Number(raw.coins) || 0);
    saved.upgrade = Math.max(0, Math.min(10, Number(raw.upgrade) || 0));
    saved.best = Math.max(0, Number(raw.best) || 0);
  } catch (err) {
    // Offline storage is optional; the game remains playable without it.
  }

  const car = { x: 0, y: 0, angle: 0, speed: 0 };
  const input = { left: false, right: false, brake: false };
  const game = {
    mode: "menu", phase: "pickup", goal: { x: ROAD_SPACING, y: 0 },
    jobs: 0, score: 0, time: 0, totalTime: 0, mission: "PICK UP THE CHICKEN",
    coins: saved.coins, upgrade: saved.upgrade, best: saved.best
  };
  const destinations = ["VIP CHICKEN", "EGG EXPRESS", "SEED RUN", "THE GOLDEN HEN", "FEATHER FAST"];
  const clamp = (n, lo, hi) => Math.min(hi, Math.max(lo, n));
  const dist = (a, b) => Math.hypot(a.x - b.x, a.y - b.y);
  const rand = (min, max) => Math.random() * (max - min) + min;
  const tint = (n, a, b) => Math.sin(n * a + b) * 43758.5453 % 1;

  function random01(ix, iy, seed = 0) {
    const n = Math.sin(ix * 127.1 + iy * 311.7 + seed * 78.23) * 43758.5453;
    return n - Math.floor(n);
  }

  function resize() {
    w = Math.max(260, window.innerWidth);
    h = Math.max(360, window.innerHeight);
    pixelRatio = Math.min(2.2, window.devicePixelRatio || 1);
    canvas.width = Math.round(w * pixelRatio);
    canvas.height = Math.round(h * pixelRatio);
    ctx.setTransform(pixelRatio, 0, 0, pixelRatio, 0, 0);
  }
  window.addEventListener("resize", resize);
  resize();

  function saveProgress() {
    try {
      localStorage.setItem(SAVE_KEY, JSON.stringify({
        coins: game.coins, upgrade: game.upgrade, best: game.best
      }));
    } catch (err) { /* best-effort persistence */ }
  }

  function roundRect(x, y, width, height, radius, color) {
    const r = Math.max(0, Math.min(radius, width / 2, height / 2));
    ctx.beginPath();
    ctx.moveTo(x + r, y);
    ctx.lineTo(x + width - r, y);
    ctx.quadraticCurveTo(x + width, y, x + width, y + r);
    ctx.lineTo(x + width, y + height - r);
    ctx.quadraticCurveTo(x + width, y + height, x + width - r, y + height);
    ctx.lineTo(x + r, y + height);
    ctx.quadraticCurveTo(x, y + height, x, y + height - r);
    ctx.lineTo(x, y + r);
    ctx.quadraticCurveTo(x, y, x + r, y);
    ctx.closePath();
    ctx.fillStyle = color;
    ctx.fill();
  }

  function beep(frequency = 590) {
    try {
      const AudioCtx = window.AudioContext || window.webkitAudioContext;
      if (!AudioCtx) return;
      soundContext = soundContext || new AudioCtx();
      if (soundContext.state === "suspended") soundContext.resume();
      const oscillator = soundContext.createOscillator();
      const gain = soundContext.createGain();
      oscillator.type = "sine";
      oscillator.frequency.value = frequency;
      gain.gain.setValueAtTime(.12, soundContext.currentTime);
      gain.gain.exponentialRampToValueAtTime(.001, soundContext.currentTime + .14);
      oscillator.connect(gain);
      gain.connect(soundContext.destination);
      oscillator.start();
      oscillator.stop(soundContext.currentTime + .15);
    } catch (err) { /* Sound is optional */ }
  }

  function showToast(message, seconds = 2) {
    refs.toast.textContent = message;
    toastTime = seconds;
  }

  function randomGoal(minBlocks = 2, maxBlocks = 4) {
    const ox = Math.round(car.x / ROAD_SPACING);
    const oy = Math.round(car.y / ROAD_SPACING);
    for (let i = 0; i < 40; i++) {
      const dx = Math.round(rand(-maxBlocks, maxBlocks));
      const dy = Math.round(rand(-maxBlocks, maxBlocks));
      if (Math.hypot(dx, dy) < minBlocks) continue;
      return { x: (ox + dx) * ROAD_SPACING, y: (oy + dy) * ROAD_SPACING };
    }
    return { x: (ox + 2) * ROAD_SPACING, y: oy * ROAD_SPACING };
  }

  function nextPickup() {
    game.phase = "pickup";
    game.goal = randomGoal(1.5, 2.8);
    game.mission = "PICK UP THE CHICKEN";
    game.time = 0;
    updateHud();
  }

  function pickUp() {
    game.phase = "delivery";
    game.goal = randomGoal(2, 3.8);
    game.mission = destinations[Math.floor(rand(0, destinations.length))];
    game.totalTime = Math.max(26, Math.ceil(dist(car, game.goal) / 76 + 28 - Math.min(game.jobs, 12) * .45));
    game.time = game.totalTime;
    showToast("🐔 CHICKEN ON BOARD!");
    beep(730);
    if (navigator.vibrate) navigator.vibrate(35);
    updateHud();
  }

  function completed() {
    game.phase = "pickup";
    game.jobs++;
    const bonus = Math.max(0, Math.ceil(game.time));
    const reward = 15 + Math.min(20, Math.floor(game.jobs / 3) * 2);
    game.score += 100 + bonus * 3;
    game.coins += reward;
    if (game.score > game.best) game.best = game.score;
    saveProgress();
    showToast("DELIVERED! +" + reward + " 🪙", 2.7);
    beep(1080);
    if (navigator.vibrate) navigator.vibrate([40, 45, 80]);
    nextPickup();
  }

  function failed() {
    showToast("TIME UP! TRY THE NEXT RIDE", 2.8);
    nextPickup();
  }

  function updateHud() {
    refs.coins.textContent = String(game.coins);
    refs.score.textContent = String(game.score);
    refs.missionTitle.textContent = game.mission;
    refs.missionSubtitle.textContent =
      game.phase === "pickup" ? "Reach the yellow chicken 🐔" : "Deliver your passenger to green!";
    refs.missionTimer.textContent = game.phase === "delivery" ? Math.ceil(game.time) + "s" : "";
    refs.speedLabel.textContent = "TAXI LVL " + (game.upgrade + 1);
    refs.upgrade.textContent = game.upgrade >= 10 ? "MAX LEVEL" : "UPGRADE · " + (30 + game.upgrade * 25) + " 🪙";
    refs.upgrade.disabled = game.upgrade >= 10;
    refs.upgrade.style.opacity = game.upgrade >= 10 ? ".55" : "1";
    refs.distance.textContent = Math.round(dist(car, game.goal)) + "m";
    const dx = game.goal.x - car.x, dy = game.goal.y - car.y;
    refs.direction.style.transform = "rotate(" + (Math.atan2(dx, -dy) * 180 / Math.PI) + "deg)";
    refs.direction.style.color = game.phase === "pickup" ? "#ffdc45" : "#5af5c2";
  }

  function setMode(mode) {
    game.mode = mode;
    refs.overlay.classList.toggle("hidden", mode === "playing");
    refs.hud.classList.toggle("hidden", mode !== "playing");
    refs.menu.classList.toggle("hidden", mode !== "menu");
    refs.pauseMenu.classList.toggle("hidden", mode !== "paused");
    Object.keys(input).forEach(k => input[k] = false);
    document.querySelectorAll(".drive").forEach(b => b.classList.remove("pressed"));
  }

  function play() {
    car.x = 0; car.y = 0; car.angle = 0; car.speed = 0;
    game.score = 0; game.jobs = 0;
    game.phase = "pickup";
    nextPickup();
    setMode("playing");
    showToast("FOLLOW THE YELLOW CHICKEN!", 2.4);
    beep(660);
  }

  function pause() {
    if (game.mode === "playing") setMode("paused");
  }

  function resume() {
    if (game.mode === "paused") {
      setMode("playing");
      lastFrame = performance.now();
    }
  }

  function upgrade() {
    if (game.mode !== "playing" || game.upgrade >= 10) return;
    const cost = 30 + game.upgrade * 25;
    if (game.coins < cost) { showToast("NEED " + cost + " COINS"); beep(220); return; }
    game.coins -= cost;
    game.upgrade++;
    saveProgress();
    showToast("TAXI LEVEL " + (game.upgrade + 1) + "!");
    beep(1000);
    updateHud();
  }

  function attachHold(id, key) {
    const node = $(id);
    const end = event => {
      event.preventDefault();
      input[key] = false;
      node.classList.remove("pressed");
      try { node.releasePointerCapture(event.pointerId); } catch (err) {}
    };
    node.addEventListener("pointerdown", event => {
      event.preventDefault();
      input[key] = true;
      node.classList.add("pressed");
      try { node.setPointerCapture(event.pointerId); } catch (err) {}
    });
    node.addEventListener("pointerup", end);
    node.addEventListener("pointercancel", end);
    node.addEventListener("lostpointercapture", () => {
      input[key] = false;
      node.classList.remove("pressed");
    });
  }
  attachHold("left", "left");
  attachHold("right", "right");
  attachHold("brake", "brake");

  $("play").addEventListener("click", play);
  $("pause").addEventListener("click", pause);
  $("resume").addEventListener("click", resume);
  $("restart").addEventListener("click", play);
  $("home").addEventListener("click", () => setMode("menu"));
  $("upgrade").addEventListener("click", upgrade);

  const keyMap = {
    ArrowLeft: "left", KeyA: "left",
    ArrowRight: "right", KeyD: "right",
    Space: "brake", ArrowDown: "brake", KeyS: "brake"
  };
  window.addEventListener("keydown", event => {
    const k = keyMap[event.code];
    if (k) { event.preventDefault(); input[k] = true; }
    if (event.code === "Escape") { event.preventDefault(); game.mode === "playing" ? pause() : resume(); }
    if (event.code === "Enter" && game.mode === "menu") play();
  });
  window.addEventListener("keyup", event => {
    const k = keyMap[event.code];
    if (k) { event.preventDefault(); input[k] = false; }
  });
  window.addEventListener("blur", pause);
  document.addEventListener("visibilitychange", () => { if (document.hidden) pause(); });
  window.chickenTaxiBack = function () {
    if (game.mode === "playing") pause();
    else if (game.mode === "paused") resume();
  };
  window.chickenTaxiPause = pause;

  function update(dt) {
    if (game.mode !== "playing") return;
    const maxSpeed = 140 + game.upgrade * 15;
    const wanted = input.brake ? 35 : maxSpeed;
    const acceleration = input.brake ? 360 : 120;
    car.speed += clamp(wanted - car.speed, -acceleration * dt, acceleration * dt);
    let turn = (input.right ? 1 : 0) - (input.left ? 1 : 0);
    car.angle += turn * (input.brake ? 3.9 : 2.35) * dt;
    car.x += Math.sin(car.angle) * car.speed * dt;
    car.y -= Math.cos(car.angle) * car.speed * dt;

    if (dist(car, game.goal) < 43) {
      if (game.phase === "pickup") pickUp();
      else completed();
    }
    if (game.phase === "delivery") {
      game.time -= dt;
      if (game.time <= 0) failed();
    }
    if (toastTime > 0) {
      toastTime -= dt;
      if (toastTime <= 0) refs.toast.textContent = "";
    }
    hudClock += dt;
    if (hudClock >= .10) {
      updateHud();
      hudClock = 0;
    }
  }

  function drawWorld(t) {
    const ox = w / 2 - car.x;
    const oy = h * .55 - car.y;
    const left = car.x - w / 2 - ROAD_SPACING * 1.3;
    const right = car.x + w / 2 + ROAD_SPACING * 1.3;
    const top = car.y - h * .55 - ROAD_SPACING * 1.3;
    const bottom = car.y + h * .45 + ROAD_SPACING * 1.3;
    const ix0 = Math.floor(left / ROAD_SPACING), ix1 = Math.ceil(right / ROAD_SPACING);
    const iy0 = Math.floor(top / ROAD_SPACING), iy1 = Math.ceil(bottom / ROAD_SPACING);
    ctx.fillStyle = "#74aa59";
    ctx.fillRect(0, 0, w, h);
    ctx.save();
    ctx.translate(ox, oy);

    // Stable procedural blocks: geometry follows camera without needing map downloads.
    for (let ix = ix0; ix < ix1; ix++) {
      for (let iy = iy0; iy < iy1; iy++) {
        const px = ix * ROAD_SPACING + ROAD_WIDTH / 2 + 5;
        const py = iy * ROAD_SPACING + ROAD_WIDTH / 2 + 5;
        const bw = ROAD_SPACING - ROAD_WIDTH - 10;
        const seed = random01(ix, iy);
        roundRect(px, py, bw, bw, 12, seed > .65 ? "#80b967" : "#8fc078");
        roundRect(px + 8, py + 8, bw - 16, bw - 16, 9, "#6fa759");
        const bx = px + 22, by = py + 20;
        const bW = bw - 43, bH = bw - 46;
        const building = seed > .50 ? "#f5d3a0" : seed > .22 ? "#c9d5d3" : "#edb6ab";
        roundRect(bx + 4, by + 7, bW, bH, 5, "#356441a8");
        roundRect(bx, by, bW, bH, 5, building);
        roundRect(bx + 10, by + 10, bW - 20, bH - 20, 3, seed > .5 ? "#cf956f" : "#9fb3ac");
        // Air conditioner/window and a small tree in every block.
        roundRect(bx + bW / 2 - 12, by + bH / 2 - 10, 24, 20, 4, "#7e9ca9");
        ctx.fillStyle = "#3d915c";
        ctx.beginPath();
        ctx.arc(px + bw - 14, py + bw - 16, 11, 0, Math.PI * 2);
        ctx.fill();
      }
    }

    // Roads: a seamless crossroads grid.
    ctx.fillStyle = "#404a53";
    for (let ix = ix0; ix <= ix1; ix++) {
      const x = ix * ROAD_SPACING;
      ctx.fillRect(x - ROAD_WIDTH / 2, top, ROAD_WIDTH, bottom - top);
    }
    for (let iy = iy0; iy <= iy1; iy++) {
      const y = iy * ROAD_SPACING;
      ctx.fillRect(left, y - ROAD_WIDTH / 2, right - left, ROAD_WIDTH);
    }
    ctx.lineWidth = 2;
    ctx.strokeStyle = "#636b73";
    for (let ix = ix0; ix <= ix1; ix++) {
      const x = ix * ROAD_SPACING;
      ctx.beginPath(); ctx.moveTo(x - ROAD_WIDTH / 2 + 3, top); ctx.lineTo(x - ROAD_WIDTH / 2 + 3, bottom);
      ctx.moveTo(x + ROAD_WIDTH / 2 - 3, top); ctx.lineTo(x + ROAD_WIDTH / 2 - 3, bottom); ctx.stroke();
    }
    for (let iy = iy0; iy <= iy1; iy++) {
      const y = iy * ROAD_SPACING;
      ctx.beginPath(); ctx.moveTo(left, y - ROAD_WIDTH / 2 + 3); ctx.lineTo(right, y - ROAD_WIDTH / 2 + 3);
      ctx.moveTo(left, y + ROAD_WIDTH / 2 - 3); ctx.lineTo(right, y + ROAD_WIDTH / 2 - 3); ctx.stroke();
    }
    ctx.fillStyle = "#e9d58d";
    for (let ix = ix0; ix <= ix1; ix++) {
      const x = ix * ROAD_SPACING;
      for (let iy = iy0; iy <= iy1; iy++) {
        for (let m = 45; m < ROAD_SPACING - 25; m += 39)
          ctx.fillRect(x - 1.5, iy * ROAD_SPACING + m, 3, 17);
      }
    }
    for (let iy = iy0; iy <= iy1; iy++) {
      const y = iy * ROAD_SPACING;
      for (let ix = ix0; ix <= ix1; ix++) {
        for (let m = 45; m < ROAD_SPACING - 25; m += 39)
          ctx.fillRect(ix * ROAD_SPACING + m, y - 1.5, 17, 3);
      }
    }

    drawMarker(t);
    ctx.restore();
    drawTaxi(w / 2, h * .55, car.angle, game.phase === "delivery");
    drawVignette();
  }

  function drawMarker(t) {
    const x = game.goal.x, y = game.goal.y;
    const pulse = 1 + Math.sin(t * 4.6) * .08;
    const pickup = game.phase === "pickup";
    const highlight = pickup ? "#ffd14a" : "#52ffb2";
    ctx.save();
    ctx.translate(x, y);
    ctx.strokeStyle = highlight;
    ctx.lineWidth = 5;
    ctx.globalAlpha = .45;
    ctx.beginPath(); ctx.arc(0, 0, 43 * pulse, 0, Math.PI * 2); ctx.stroke();
    ctx.globalAlpha = 1;
    ctx.fillStyle = pickup ? "#fff4c8" : "#9dfcd6";
    ctx.beginPath(); ctx.arc(0, 0, 28, 0, Math.PI * 2); ctx.fill();
    ctx.lineWidth = 4;
    ctx.strokeStyle = pickup ? "#c58c08" : "#0a8a5e";
    ctx.stroke();
    if (pickup) {
      ctx.fillStyle = "#ffca2a";
      ctx.beginPath(); ctx.arc(0, 1, 21, 0, Math.PI * 2); ctx.fill();
      ctx.fillStyle = "#e65b4d";
      for (let i = -1; i <= 1; i++) {
        ctx.beginPath(); ctx.arc(i * 7, -20, 6, 0, Math.PI * 2); ctx.fill();
      }
      ctx.fillStyle = "#19212c";
      ctx.beginPath(); ctx.arc(-8, -3, 3, 0, Math.PI * 2); ctx.fill();
      ctx.beginPath(); ctx.arc(8, -3, 3, 0, Math.PI * 2); ctx.fill();
      ctx.fillStyle = "#ff8530";
      ctx.beginPath(); ctx.moveTo(-7, 5); ctx.lineTo(7, 5); ctx.lineTo(0, 13); ctx.closePath(); ctx.fill();
    } else {
      ctx.strokeStyle = "#137e52";
      ctx.lineWidth = 6;
      ctx.lineCap = "round";
      ctx.beginPath(); ctx.moveTo(-13, 1); ctx.lineTo(-3, 11); ctx.lineTo(16, -12); ctx.stroke();
    }
    ctx.restore();
  }

  function drawTaxi(x, y, angle, passenger) {
    ctx.save();
    ctx.translate(x, y);
    ctx.rotate(angle);
    ctx.fillStyle = "#12253055";
    roundRect(-21, -30, 45, 71, 9, "#12253055");
    roundRect(-23, -21, 7, 18, 2, "#20262b");
    roundRect(16, -21, 7, 18, 2, "#20262b");
    roundRect(-23, 16, 7, 17, 2, "#20262b");
    roundRect(16, 16, 7, 17, 2, "#20262b");
    roundRect(-18, -34, 36, 70, 9, "#ffcb38");
    roundRect(-13, -23, 26, 18, 3, "#4f9eb4");
    roundRect(-13, 17, 26, 12, 3, "#2a5d79");
    roundRect(-10, -1, 20, 12, 4, passenger ? "#fff3b9" : "#fff9e8");
    ctx.fillStyle = passenger ? "#e0a113" : "#5c5344";
    ctx.font = "bold 9px system-ui";
    ctx.textAlign = "center";
    ctx.fillText(passenger ? "🐔" : "TAXI", 0, 8);
    roundRect(-13, -36, 10, 5, 2, "#fff6c8");
    roundRect(3, -36, 10, 5, 2, "#fff6c8");
    roundRect(-12, 33, 8, 3, 1, "#e66342");
    roundRect(4, 33, 8, 3, 1, "#e66342");
    ctx.restore();
  }

  function drawVignette() {
    const grd = ctx.createRadialGradient(w * .5, h * .54, h * .12, w * .5, h * .54, h * .88);
    grd.addColorStop(0, "#071b2700");
    grd.addColorStop(1, "#071b2760");
    ctx.fillStyle = grd;
    ctx.fillRect(0, 0, w, h);
  }

  function frame(timestamp) {
    if (!lastFrame) lastFrame = timestamp;
    const delta = clamp((timestamp - lastFrame) / 1000, 0, .05);
    lastFrame = timestamp;
    update(delta);
    ctx.setTransform(pixelRatio, 0, 0, pixelRatio, 0, 0);
    drawWorld(timestamp / 1000);
    requestAnimationFrame(frame);
  }

  updateHud();
  setMode("menu");
  requestAnimationFrame(frame);
})();
