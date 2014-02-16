var selector = null;

calendarSelector = function () {
    var self = this;
    var greenDays = [];
    var redDays = [];
    var $avaliableDates = $('#avaliable');

    function getMeetingId() {
        return $('#MeetingId').val();
    }

    function getUserName() {
        return $('#UserName').val();
    }

    this.dayClicked = function (date, allDay, jsEvent, view) {
        var avaliableDates = $avaliableDates.is(':checked');
        var isoDate = date.toISOString();
        self.markDate(date, isoDate, avaliableDates, $(this));
    };

    this.markDate = function (date, isoDate, isAvaliable, cell) {
        if (greenDays[date] && isAvaliable) {
            greenDays[date] = null;
            cell.css('background-color', '');
        } else if (redDays[date] && !isAvaliable) {
            redDays[date] = null;
            cell.css('background-color', '');
        } else {
            if (isAvaliable) {
                greenDays[date] = isoDate;
                redDays[date] = null;
                cell.css('background-color', 'green');
            } else {
                greenDays[date] = null;
                redDays[date] = isoDate;
                cell.css('background-color', '#E96A6D');
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
                meetingId: getMeetingId(),
                userName: getUserName()
            }),
            contentType: "application/json; charset=utf-8",
            dataType: "json"
        }).done(function (saveResult) {
            if (saveResult == 1) {
                $("#calendar").hide();
                $('#radioButtons').hide();
                $('#sendButton').hide();
                $("#resultLink").show();
                $('#UserName').hide();
                $('#lblEnterName').hide();
            }
        });
    };

    this.gotoResult = function() {
        var url = "/Result/Index/" + getMeetingId();
        window.location.assign(url);
    };
};