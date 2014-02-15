var selector = null;

calendarSelector = function () {
    var self = this;
    var greenDays = [];
    var redDays = [];
    var $avaliableDates = $('#avaliable');

    function getMeetingId() {
        return $('#MeetingId').val();
    }

    this.dayClicked = function (date, allDay, jsEvent, view) {
        var isoDate = date.toISOString();
        var avaliableDates = $avaliableDates.is(':checked');
        if (greenDays[date] && avaliableDates) {
            greenDays[date] = null;
            $(this).css('background-color', '');
        } else if (redDays[date] && !avaliableDates) {
            redDays[date] = null;
            $(this).css('background-color', '');
        } else {
            if (avaliableDates) {
                greenDays[date] = isoDate;
                redDays[date] = null;
                $(this).css('background-color', 'green');
            } else {
                greenDays[date] = null;
                redDays[date] = isoDate;
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
                $('#radioButtons').hide();
                $('#sendButton').hide();
                $("#resultLink").show();
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
        height: 320,
        width: 480,
        dayClick: selector.dayClicked,
    });
});