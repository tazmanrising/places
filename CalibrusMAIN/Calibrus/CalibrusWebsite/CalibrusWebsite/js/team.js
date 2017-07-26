$(function () {

    //expand content
    $('.toggle-bio').click(function (e) {

        //toggle expandible content
        if ($(this).hasClass("james")) {
            $('.james.full-bio').fadeToggle('slow');
        }
        if ($(this).hasClass("gary")) {
            $('.gary.full-bio').fadeToggle('slow');
        }
        if ($(this).hasClass("tom")) {
            $('.tom.full-bio').fadeToggle('slow');
        }
        if ($(this).hasClass("craig")) {
            $('.craig.full-bio').fadeToggle('slow');
        }
        if ($(this).hasClass("michael")) {
            $('.michael.full-bio').fadeToggle('slow');
        }
        if ($(this).hasClass("kelly")) {
            $('.kelly.full-bio').fadeToggle('slow');
        }
        if ($(this).hasClass("tamara")) {
            $('.tamara.full-bio').fadeToggle('slow');
        }

        //change link text
        if ($(this).text() == 'show more...') {
            $(this).text('show less...');
        } else {
            $(this).text('show more...');
        }
        return false;
    });
});