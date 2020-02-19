$(document).ready(function () {

    $("#RecurrenceType").change(function () {
        var selectedId = $(this).val();
        setRecurrenceTypeVisibility(selectedId);
    });

    var selectedId = $("#RecurrenceType").val();
    setRecurrenceTypeVisibility(selectedId);

    positionFooter($(".footer"));
});

$(window).resize(function () {
    //var bodyHeight = $("body").outerHeight(true);
    //var pageHeight = $(window).height();

    //$("#bodyHeight").text(bodyHeight);
    //$("#pageHeight").text(pageHeight);

    positionFooter($(".footer"));
});

function setRecurrenceTypeVisibility(index) {
    $("div[id$='-rec']").hide();

    switch(index) {
    case "1":
        $("#daily-rec").show();
        break;
    case "2":
        $("#weekly-rec").show();
        break;
    case "3":
        $("#biWeeklyOdd-rec").show();
        break;
    case "4":
        $("#biWeeklyEven-rec").show();
        break;
    case "5":
        $("#biMonthly-rec").show();
        break;
    case "6":
        $("#monthly-rec").show();
        break;
    case "7":
        $("#quarterly-rec").show();
        break;
    case "8":
        $("#biYearly-rec").show();
        break;
    case "9":
        $("#yearly-rec").show();
        break;
    case "10":
        $("#yearly-rec").show();
        break;
    }
}

function positionFooter(obj) {
    if ($("body").outerHeight(true) + 60 <= $(window).height()) {
        obj.css("position", "absolute");
        obj.css("bottom", 0);
        //$("#mode").text("Mode 1");
    } else if ($("body").outerHeight(true) === $(window).height()) {
        obj.css("position", "absolute");
        obj.css("bottom", 0);
        //$("#mode").text("Mode 3");
    } else {
        obj.css("position", "relative");
        obj.css("bottom", "auto" );
        //$("#mode").text("Mode 2");
    }
}

function submitOnEnter() {
    var user = document.getElementById("Username");
    user.addEventListener("keypress", function (event) {
        if (event.keyCode === 13) {
            document.getElementById("btnLogin").click();
        }
    });
}