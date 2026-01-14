const FitTrackUI = (() => {
  // IMPORTANT: schimbă portul dacă API-ul tău rulează pe altceva
 const API_BASE = "/api";


  async function req(method, url, body) {
    const res = await fetch(url, {
      method,
      headers: body ? { "Content-Type": "application/json" } : undefined,
      body: body ? JSON.stringify(body) : undefined
    });

    if (!res.ok) {
      let text = `${res.status} ${res.statusText}`;
      try {
        const data = await res.json();
        text = data.error ? data.error : JSON.stringify(data);
      } catch {}
      throw new Error(text);
    }

    if (res.status === 204) return null;
    return res.json();
  }

  // ---------------- Workouts API ----------------
  const workoutsApi = {
    list: () => req("GET", `${API_BASE}/workouts`),
    get: (id) => req("GET", `${API_BASE}/workouts/${id}`),
    create: (dto) => req("POST", `${API_BASE}/workouts`, dto),
    update: (id, dto) => req("PUT", `${API_BASE}/workouts/${id}`, dto),
    remove: (id) => req("DELETE", `${API_BASE}/workouts/${id}`)
  };

  // ---------------- Exercises API ----------------
  const exercisesApi = {
    list: (workoutId) => {
      const q = workoutId ? `?workoutId=${encodeURIComponent(workoutId)}` : "";
      return req("GET", `${API_BASE}/exercises${q}`);
    },
    get: (id) => req("GET", `${API_BASE}/exercises/${id}`),
    create: (dto) => req("POST", `${API_BASE}/exercises`, dto),
    update: (id, dto) => req("PUT", `${API_BASE}/exercises/${id}`, dto),
    remove: (id) => req("DELETE", `${API_BASE}/exercises/${id}`)
  };

  // ---------------- Workouts UI ----------------
  function initWorkouts() {
    const tbody = document.getElementById("workoutsTbody");
    const errBox = document.getElementById("workoutsError");

    const form = document.getElementById("workoutCreateForm");
    const reloadBtn = document.getElementById("reloadWorkouts");

    // modal elements (trebuie să existe în workouts.html)
    const overlay = document.getElementById("workoutModalOverlay");
    const closeBtn = document.getElementById("closeWorkoutModal");
    const cancelBtn = document.getElementById("cancelWorkoutEdit");
    const editForm = document.getElementById("workoutEditForm");

    // VIEW box elements
    const viewBox = document.getElementById("workoutViewBox");
    const viewTitle = document.getElementById("viewTitle");
    const viewMeta = document.getElementById("viewMeta");
    const viewErr = document.getElementById("viewError");
    const viewTbody = document.getElementById("viewExercisesTbody");
    const closeViewBtn = document.getElementById("closeView");

    // default: azi
    const dateInput = document.getElementById("wDate");
    if (dateInput && !dateInput.value) {
      dateInput.value = toDateInputValue(new Date());
    }

    function showError(msg) {
      errBox.textContent = msg;
      errBox.classList.remove("hidden");
    }

    function clearError() {
      errBox.classList.add("hidden");
      errBox.textContent = "";
    }

    function openWorkoutModal(w) {
      document.getElementById("editWorkoutId").value = w.id;
      document.getElementById("editWName").value = w.name ?? "";
      document.getElementById("editWDate").value = w.date ? w.date.slice(0, 10) : toDateInputValue(new Date());
      document.getElementById("editWNotes").value = w.notes ?? "";
      document.getElementById("editWDescription").value = w.description ?? "";
      overlay.classList.add("show");
    }

    function closeWorkoutModal() {
      overlay.classList.remove("show");
    }

    // close handlers
    if (closeBtn) closeBtn.addEventListener("click", closeWorkoutModal);
    if (cancelBtn) cancelBtn.addEventListener("click", closeWorkoutModal);
    if (overlay) {
      overlay.addEventListener("click", (e) => {
        if (e.target === overlay) closeWorkoutModal();
      });
    }

    if (editForm) {
      editForm.addEventListener("submit", async (e) => {
        e.preventDefault();
        clearError();

        const id = document.getElementById("editWorkoutId").value;
        const name = document.getElementById("editWName").value.trim();
        const date = document.getElementById("editWDate").value;
        const notes = document.getElementById("editWNotes").value.trim();
        const description = document.getElementById("editWDescription").value.trim();

        try {
          await workoutsApi.update(id, {
            name,
            date: new Date(date).toISOString(),
            notes: notes.length ? notes : null,
            description: description.length ? description : null
          });
          closeWorkoutModal();
          await render();
        } catch (err) {
          showError(err.message);
        }
      });
    }

    // ---------- VIEW helpers ----------
    function viewShowError(msg) {
      viewErr.textContent = msg;
      viewErr.classList.remove("hidden");
    }
    function viewClearError() {
      viewErr.classList.add("hidden");
      viewErr.textContent = "";
    }
    function closeView() {
      viewBox.classList.add("hidden");
      viewTbody.innerHTML = "";
      viewMeta.textContent = "";
      viewClearError();
    }
    if (closeViewBtn) closeViewBtn.addEventListener("click", closeView);

    async function openView(workoutId) {
      viewBox.classList.remove("hidden");
      viewTitle.textContent = `View workout #${workoutId}`;
      viewTbody.innerHTML = "";
      viewMeta.textContent = "";
      viewClearError();

      try {
        const list = await exercisesApi.list(workoutId);

        const totalExercises = list.length;
        const totalSets = list.reduce((s, x) => s + (Number(x.sets) || 0), 0);
        const totalReps = list.reduce((s, x) => s + ((Number(x.sets) || 0) * (Number(x.reps) || 0)), 0);
        viewMeta.textContent = `Total: ${totalExercises} exerciții • ${totalSets} seturi • ~${totalReps} repetări`;

        if (list.length === 0) {
          const tr = document.createElement("tr");
          tr.innerHTML = `<td colspan="6" class="muted">Nu există exerciții pentru acest workout.</td>`;
          viewTbody.appendChild(tr);
          return;
        }

        list.forEach(ex => {
          const tr = document.createElement("tr");
          tr.innerHTML = `
            <td>${ex.id}</td>
            <td>${escapeHtml(ex.name ?? "")}</td>
            <td>${ex.sets}</td>
            <td>${ex.reps}</td>
            <td>${ex.weightKg}</td>
            <td>${ex.restSeconds}</td>
          `;
          viewTbody.appendChild(tr);
        });
      } catch (err) {
        viewShowError(err.message);
      }
    }

    // ---------- render workouts ----------
    async function render() {
      clearError();
      tbody.innerHTML = "";
      try {
        const list = await workoutsApi.list();

        list.forEach((w, idx) => {
          const tr = document.createElement("tr");

          const date = w.date ? new Date(w.date).toLocaleDateString() : "";
          const descShort = (w.description ?? "").slice(0, 80);

          tr.innerHTML = `
            <td>${idx + 1}</td>
            <td>${w.id}</td>
            <td>${escapeHtml(w.name ?? "")}</td>
            <td>${escapeHtml(date)}</td>
            <td>${escapeHtml(w.notes ?? "")}</td>
            <td title="${escapeHtml(w.description ?? "")}">
              ${escapeHtml(descShort)}${(w.description && w.description.length > 80) ? "…" : ""}
            </td>
            <td>
              <div class="actions">
                <button class="btn" data-view="${w.id}">View</button>
                <button class="btn" data-edit="${w.id}">Edit</button>
                <button class="btn danger" data-del="${w.id}">Delete</button>
              </div>
            </td>
          `;
          tbody.appendChild(tr);
        });
      } catch (e) {
        showError(e.message);
      }
    }

    // ---------- create workout ----------
    form.addEventListener("submit", async (e) => {
      e.preventDefault();
      clearError();

      const name = document.getElementById("wName").value.trim();
      const date = document.getElementById("wDate").value;
      const notes = document.getElementById("wNotes").value.trim();
      const description = document.getElementById("wDescription").value.trim();

      try {
        await workoutsApi.create({
          name,
          date: new Date(date).toISOString(),
          notes: notes.length ? notes : null,
          description: description.length ? description : null
        });

        form.reset();
        if (dateInput) dateInput.value = date;
        closeView(); // dacă era deschis view, îl închide
        await render();
      } catch (err) {
        showError(err.message);
      }
    });

    reloadBtn.addEventListener("click", () => render());

    // ---------- row click actions ----------
    tbody.addEventListener("click", async (e) => {
      const viewId = e.target.dataset.view;
      const editId = e.target.dataset.edit;
      const delId = e.target.dataset.del;

      if (viewId) {
        await openView(viewId);
        return;
      }

      if (delId) {
        if (!confirm(`Ștergi workout #${delId}?`)) return;
        try {
          await workoutsApi.remove(delId);
          closeView();
          await render();
        } catch (err) {
          showError(err.message);
        }
        return;
      }

      if (editId) {
        try {
          const w = await workoutsApi.get(editId);
          openWorkoutModal(w);
        } catch (err) {
          showError(err.message);
        }
      }
    });

    render();
  }

  // ---------------- Exercises UI ----------------
  function initExercises() {
    const tbody = document.getElementById("exercisesTbody");
    const errBox = document.getElementById("exercisesError");

    const form = document.getElementById("exerciseCreateForm");
    const reloadBtn = document.getElementById("reloadExercises");

    const filterInput = document.getElementById("filterWorkoutId");
    const applyBtn = document.getElementById("applyFilter");
    const clearBtn = document.getElementById("clearFilter");

    // modal elements (trebuie să existe în exercises.html)
    const eOverlay = document.getElementById("exerciseModalOverlay");
    const eCloseBtn = document.getElementById("closeExerciseModal");
    const eCancelBtn = document.getElementById("cancelExerciseEdit");
    const eEditForm = document.getElementById("exerciseEditForm");

    function showError(msg) {
      errBox.textContent = msg;
      errBox.classList.remove("hidden");
    }
    function clearError() {
      errBox.classList.add("hidden");
      errBox.textContent = "";
    }

    function openExerciseModal(ex) {
      document.getElementById("editExerciseId").value = ex.id;
      document.getElementById("editEName").value = ex.name ?? "";
      document.getElementById("editESets").value = ex.sets ?? 3;
      document.getElementById("editEReps").value = ex.reps ?? 10;
      document.getElementById("editEWeightKg").value = ex.weightKg ?? 0;
      document.getElementById("editERest").value = ex.restSeconds ?? 60;
      eOverlay.classList.add("show");
    }

    function closeExerciseModal() {
      eOverlay.classList.remove("show");
    }

    if (eCloseBtn) eCloseBtn.addEventListener("click", closeExerciseModal);
    if (eCancelBtn) eCancelBtn.addEventListener("click", closeExerciseModal);
    if (eOverlay) {
      eOverlay.addEventListener("click", (e) => {
        if (e.target === eOverlay) closeExerciseModal();
      });
    }

    if (eEditForm) {
      eEditForm.addEventListener("submit", async (e) => {
        e.preventDefault();
        clearError();

        const id = document.getElementById("editExerciseId").value;
        const dto = {
          name: document.getElementById("editEName").value.trim(),
          sets: Number(document.getElementById("editESets").value),
          reps: Number(document.getElementById("editEReps").value),
          weightKg: Number(document.getElementById("editEWeightKg").value),
          restSeconds: Number(document.getElementById("editERest").value),
        };

        try {
          await exercisesApi.update(id, dto);
          closeExerciseModal();
          await render();
        } catch (err) {
          showError(err.message);
        }
      });
    }

    async function render() {
      clearError();
      tbody.innerHTML = "";
      const workoutId = filterInput.value ? Number(filterInput.value) : null;

      try {
        const list = await exercisesApi.list(workoutId);
        list.forEach(ex => {
          const tr = document.createElement("tr");
          tr.innerHTML = `
            <td>${ex.id}</td>
            <td>${ex.workoutId}</td>
            <td>${escapeHtml(ex.name ?? "")}</td>
            <td>${ex.sets}</td>
            <td>${ex.reps}</td>
            <td>${ex.weightKg}</td>
            <td>${ex.restSeconds}</td>
            <td>
              <div class="actions">
                <button class="btn" data-edit="${ex.id}">Edit</button>
                <button class="btn danger" data-del="${ex.id}">Delete</button>
              </div>
            </td>
          `;
          tbody.appendChild(tr);
        });
      } catch (e) {
        showError(e.message);
      }
    }

    form.addEventListener("submit", async (e) => {
      e.preventDefault();
      clearError();

      const dto = {
        workoutId: Number(document.getElementById("eWorkoutId").value),
        name: document.getElementById("eName").value.trim(),
        sets: Number(document.getElementById("eSets").value),
        reps: Number(document.getElementById("eReps").value),
        weightKg: Number(document.getElementById("eWeightKg").value),
        restSeconds: Number(document.getElementById("eRest").value)
      };

      try {
        await exercisesApi.create(dto);
        form.reset();
        await render();
      } catch (err) {
        showError(err.message);
      }
    });

    reloadBtn.addEventListener("click", () => render());
    applyBtn.addEventListener("click", () => render());
    clearBtn.addEventListener("click", () => { filterInput.value = ""; render(); });

    tbody.addEventListener("click", async (e) => {
      const editId = e.target.dataset.edit;
      const delId = e.target.dataset.del;

      if (delId) {
        if (!confirm(`Ștergi exercise #${delId}?`)) return;
        try {
          await exercisesApi.remove(delId);
          await render();
        } catch (err) {
          showError(err.message);
        }
      }

      if (editId) {
        try {
          const ex = await exercisesApi.get(editId);
          openExerciseModal(ex);
        } catch (err) {
          showError(err.message);
        }
      }
    });

    render();
  }

  function toDateInputValue(date) {
    const d = new Date(date);
    const yyyy = d.getFullYear();
    const mm = String(d.getMonth() + 1).padStart(2, "0");
    const dd = String(d.getDate()).padStart(2, "0");
    return `${yyyy}-${mm}-${dd}`;
  }

  function escapeHtml(str) {
    return String(str)
      .replaceAll("&", "&amp;")
      .replaceAll("<", "&lt;")
      .replaceAll(">", "&gt;")
      .replaceAll('"', "&quot;")
      .replaceAll("'", "&#039;");
  }

  return { initWorkouts, initExercises };
})();
