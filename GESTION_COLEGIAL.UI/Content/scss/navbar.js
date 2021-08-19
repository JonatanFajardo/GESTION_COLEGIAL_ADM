

!function ($) {
     
        alert('hola2'); 
        alert('hola2'); 

    $('.menu-button').click(function () {
        $sidemenu = document.getElementById('sidemenu');
        /*    margin-left: 240px; */
        if ($sidemenu.clientWidth == 75) {
            $sidemenu.style.width = "240px"
        }
        else if ($sidemenu.clientWidth == 240) {
            $sidemenu.style.width = "75px"
        }
    })

}(window.jQuery)
