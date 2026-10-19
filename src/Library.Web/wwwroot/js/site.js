// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
  document.querySelectorAll(".toast").forEach(function (el) {
    var toast = bootstrap.Toast.getOrCreateInstance(el);
    toast.show();
  });
});
