var selector = null;

calendarSelector = function () {
    var self = this;
    var greenDays = [];
    var redDays = [];
    var $avaliableDates = $('#avaliableDates');

    function convertDateToSimpleDate(date, month) {
        var simpleDate = {
            Month: month,
            Year: date.getUTCFullYear(),
            Day: date.getDate()
        };
        return simpleDate;
    }
    
    function getMeetingId() {
        return $('#MeetingId').val();
    }

    this.dayClicked = function (date, allDay, jsEvent, view) {
        var month;
        var title = view.title;
        var monthNames = view.calendar.options.monthNames;
        $.each(monthNames, function(index, elem) {
            if (title.substring(0, elem.length) == elem)
                month = index + 1;
        });
        
        var avaliableDates = $avaliableDates.is(':checked');
        if (greenDays[date] && avaliableDates) {
            greenDays[date] = null;
            $(this).css('background-color', '');
        } else if (redDays[date] && !avaliableDates) {
            redDays[date] = null;
            $(this).css('background-color', '');
        } else {
            if (avaliableDates) {
                greenDays[date] = convertDateToSimpleDate(date, month);
                redDays[date] = null;
                $(this).css('background-color', 'green');
            } else {
                greenDays[date] = null;
                redDays[date] = convertDateToSimpleDate(date, month);
                $(this).css('background-color', 'red');
            }
        }
    };

    this.sendForCount = function() {
        $.ajax({
            type: "POST",
            url: "/Meeting/CountResult",
            data: JSON.stringify({
                meetingId: getMeetingId()
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).done(function (saveResult) {
            if (saveResult == 1) {
                self.gotoResult();
            }
        });
    };

    this.sendClicked = function () {
        var green = [];
        var red = [];
        for (date in greenDays) {
            if (greenDays[date]) {
                green.push(greenDays[date]);
            }
        }
        for (date in redDays) {
            if (redDays[date])
                red.push(redDays[date]);
        }
        $.ajax({
            type: "POST",
            url: "/Meeting/SendResults",
            data: JSON.stringify({
                greenDays: green,
                redDays: red,
                meetingId: getMeetingId()
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).done(function (saveResult) {
            if (saveResult == 1) {
                $("#calendar").hide();
                $('.edit-radiobuttons').hide();
                $('.edit-description').hide();
                $("#resultLink").show();
                $('#sendButton').hide();
            }
        });
    };

    this.gotoResult = function() {
        var url = "/Result/Index/" + getMeetingId();
        window.location.assign(url);
    };
};

$(document).ready(function () {
    selector = new calendarSelector();
    $('#calendar').fullCalendar({
        editable: true,
        dayClick: selector.dayClicked,
    });
});