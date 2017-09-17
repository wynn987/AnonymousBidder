function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
          .toString(16)
          .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
      s4() + '-' + s4() + s4() + s4();
}

$(document).ready(function () {
    $('ul.nav > li').off().on('click', 'a', function () {

        $('ul.nav > li').each(function () {
            if ($(this).hasClass('current')) {
                $(this).removeClass('current');
            }
        });

        var litag = $(this).parent().parent().parent();
        if (litag.hasClass('current')) {
            litag.removeClass('current');
        } else {
            litag.addClass('current');
        }
    });
});

function GetDateFromIntOrString(Value) {
    if (Value != null && Value != "" && Value.indexOf("Date") != -1) {
        var IntValue = parseInt(Value.substr(6));
        if (IntValue <= 0) {
            return "";
        }
        var date = new Date(IntValue);
        var dd = "";
        if (date.getDate() < 10) {
            dd = '0' + date.getDate();
        }
        else {
            dd = date.getDate();
        }
        var mm = "";
        if ((date.getMonth() + 1) < 10) {
            mm = '0' + (date.getMonth() + 1);
        }
        else {
            mm = (date.getMonth() + 1);
        }
        return dd + "/" + mm + "/" + date.getFullYear();
    }
    else if (Value != null && Value != "" && Value.indexOf("T") != -1) {
        var fulldate = moment(Value)._d;
        return moment(fulldate).format('DD/MM/YYYY');
    }
    else {
        return Value == null ? "" : Value;
    }

}


/*Common Function*/
function gridDataBound(e) {
    //debugger;
    var grid = e.sender;
    if (grid.dataSource.total() == 0) {
        var colCount = grid.columns.length;
        $(e.sender.wrapper)
            .find('tbody')
            .append('<tr class="kendo-data-row"><td colspan="' + colCount + '" class="no-data">No data found.</td></tr>');
    }
};

var disableButton = function ($selector) {
    $selector.attr("disabled", "disabled");
    $selector.removeClass("btn-u");
    $selector.addClass("btn");
};

var enableButton = function ($selector) {
    $selector.removeAttr("disabled");
    $selector.removeClass("btn");
    $selector.addClass("btn-u");
};


function onHistoryBack() {
    history.back();
}

function onWindowOpen() {
    $('#kWindow').html('<div style="text-align: center; position: absolute; top: 50%; left: 0px; width: 100%; height: 1px; overflow: visible; display: block;"><div style="margin-left: -125px; position: absolute; top: -35px; left: 50%; width: 250px; height: 70px;"><img class="img-loading" src="/Content/CSS/kendo/Default/loading.gif" /></div></div>');
}

function onNewWindowOpen() {
    $('#kWindow1').html('<div style="text-align: center; position: absolute; top: 50%; left: 0px; width: 100%; height: 1px; overflow: visible; display: block;"><div style="margin-left: -125px; position: absolute; top: -35px; left: 50%; width: 250px; height: 70px;"><img class="img-loading" src="/Content/CSS/kendo/Default/loading.gif" /></div></div>');
}

function showKendoDialog(title, content, width, height, modal, maximize) {
    var $window = $("#kWindow").kendoWindow({
        content: {
            url: content
        },
        modal: modal,
        actions: [
             //"Maximize",
             "Close"
        ],
        close: onClose,
        open: onWindowOpen
    }).data("kendoWindow");

    $window.setOptions({
        width: width,
        height: height,
    });

    $window.title(title);
    $window.center(true).open();
    if (maximize) {
        $window.maximize();
    }

    var onClose = function () {
        $window.destroy();
    }
}


function showNewKendoDialog(title, content, width, height, modal, maximize) {
    var $window = $("#kWindow1").kendoWindow({
        content: {
            url: content
        },
        modal: modal,
        actions: [
             "Maximize",
             "Close"
        ],
        close: onClose,
        open: onNewWindowOpen
    }).data("kendoWindow");

    $window.setOptions({
        width: width,
        height: height,
    });

    $window.title(title);
    $window.center(true).open();

    if (maximize) {
        $window.maximize();
    }
    var onClose = function () {
        $window.destroy();
    }
}

function closeKendoWindow(sender) {
    if (sender == undefined)
        $("#kWindow").data("kendoWindow").close();
    else
        $(sender).closest(".k-window-content").data("kendoWindow").close();
}

function openNewWindow(url) {
    var windowObjectReference = window.open(url, "SecondScreen", "directories=no,titlebar=no,toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=no");//", width=" + $(document).width() + ",height=" + $(document).height());
    setTimeout(function () {
        windowObjectReference.focus();
    }, 400);
}

function error_handler(e) {
    if (e.errors) {
        var message = "";

        $.each(e.errors, function (key, value) {
            if ('errors' in value) {
                $.each(value.errors, function () {
                    message += "<li>" + this + "</li>";
                });
            }
        });
        Feedback.ShowError(message);
    }
}

(function ($) {
    $.QueryString = (function (a) {
        if (a == "") return {};
        var b = {};
        for (var i = 0; i < a.length; ++i) {
            var p = a[i].split('=');
            if (p.length != 2) continue;
            b[p[0]] = decodeURIComponent(p[1].replace(/\+/g, " "));
        }
        return b;
    })(window.location.search.substr(1).split('&'));
})(jQuery);

function escapeRegExp(string) {
    return string.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}

function replaceAll(string, find, replace) {
    return string.replace(new RegExp(escapeRegExp(find), 'g'), replace);
}


/*Loading icon*/
function ShowLoading(target) {
    var element = $(target);
    kendo.ui.progress(element, true);
}


function HideLoading(target) {
    var element = $(target);
    setTimeout(function () {
        kendo.ui.progress(element, false);
    }, 1000);
}
/*End loading icon*/
