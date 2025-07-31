window.initializeSidebarToggle = function () {
    setTimeout(function () {
        $('.sidebar-toggler').off('click').on('click', function () {
            $('.sidebar, .content').toggleClass("open");
            return false;
        });
    }, 50); // Espera 50ms para asegurarse de que el DOM esté listo
};