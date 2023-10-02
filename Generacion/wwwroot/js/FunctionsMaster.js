document.addEventListener("DOMContentLoaded", function () {
    const checkboxes = document.querySelectorAll('input[type="checkbox"].ocultar-columna');

    checkboxes.forEach(function (checkbox) {
        checkbox.addEventListener("change", function () {
            const columnIndex = checkbox.dataset.columna;
            const columnasTabla = document.querySelectorAll(`#CuentaContable tr td:nth-child(${parseInt(columnIndex) + 1})`);
            const cabeceraTabla = document.querySelectorAll(`#CuentaContable th:nth-child(${parseInt(columnIndex) + 1})`);

            if (checkbox.checked) {
                columnasTabla.forEach(function (col) {
                    col.style.display = "none";
                });

                cabeceraTabla.forEach(function (th) {
                    th.style.display = "none"; 
                });
            } else {
                columnasTabla.forEach(function (col) {
                    col.style.display = ""; 
                });

                cabeceraTabla.forEach(function (th) {
                    th.style.display = ""; 
                });
            }
        });
    });
});
$(document).ready(function () {
    $('#CuentaContable').DataTable({
        "lengthMenu": [10, 20, 50, -1], 
        "pageLength": 10 
    });
});