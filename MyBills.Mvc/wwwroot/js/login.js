$(document).ready(function () {
    $("form input").keypress(function (e) {
        if ((e.which && e.which === 13) || (e.keyCode && e.keyCode === 13)) {
            var submit = $("input[type=submit]");
            submit.click();
        }
    });
});