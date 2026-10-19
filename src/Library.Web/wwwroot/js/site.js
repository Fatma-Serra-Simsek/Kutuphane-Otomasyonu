document.addEventListener("DOMContentLoaded", function () {
  document.querySelectorAll(".toast").forEach(function (el) {
    var toast = bootstrap.Toast.getOrCreateInstance(el);
    toast.show();
  });

  var sidebar = document.getElementById("appSidebar");
  var toggle = document.getElementById("sidebarToggle");
  var backdrop = document.getElementById("sidebarBackdrop");

  function closeSidebar() {
    if (!sidebar) return;
    sidebar.classList.remove("is-open");
    if (backdrop) backdrop.classList.remove("is-visible");
  }

  function openSidebar() {
    if (!sidebar) return;
    sidebar.classList.add("is-open");
    if (backdrop) backdrop.classList.add("is-visible");
  }

  if (toggle && sidebar) {
    toggle.addEventListener("click", function () {
      if (sidebar.classList.contains("is-open")) closeSidebar();
      else openSidebar();
    });
  }

  if (backdrop) {
    backdrop.addEventListener("click", closeSidebar);
  }

  document.querySelectorAll("img[data-cover-fallback]").forEach(function (img) {
    img.addEventListener("error", function () {
      img.style.display = "none";
      var placeholder = img.parentElement && img.parentElement.querySelector(".book-cover-placeholder");
      if (placeholder) placeholder.hidden = false;
    });
  });
});
