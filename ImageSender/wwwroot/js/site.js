// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $(".datatable").DataTable({
        "paging": true,
        "lengthMenu": [[8, 16, 24, -1], [8, 16, 24, "All"]],
        "pageLength": 8,
    });
}