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

    function markCellRed(cell) {
        cell.css('background-color', '#E96A6D');
    }
    
    function markCellGreen(cell) {
        cell.css('background-color', 'green');
    }

    function resetCellColor(cell) {
        cell.css('background-color', '');
    };

    this.dayClicked = function (date, allDay, jsEvent, view) {
        var avaliableDates = $avaliableDates.is(':checked');
        self.markDate(date, avaliableDates, $(this));
    };

    this.dayRender = function(date, cell) {
        if (greenDays[date]) {
            markCellGreen(cell);
        }
        if (redDays[date]) {
            markCellRed(cell);
        }
    };

    this.markDate = function (date, isAvaliable, cell) {
        if (greenDays[date] && isAvaliable) {
            greenDays[date] = null;
            resetCellColor(cell);
        } else if (redDays[date] && !isAvaliable) {
            redDays[date] = null;
            resetCellColor(cell);
        } else {
            if (isAvaliable) {
                self.addGreenDay(date);;
                redDays[date] = null;
                markCellGreen(cell);
            } else {
                greenDays[date] = null;
                self.addRedDay(date);
                markCellRed(cell);
            }
        }
    };

    this.addGreenDay = function(date) {
        greenDays[date] = date.toDateString();
    };
    this.addRedDay = function (date) {
        redDays[date] = date.toDateString();
    };

    this.monthChanged = function (e) {
        $('#calendar').fullCalendar('render');
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
            //if (saveResult == 1) {
            //    $("#calendar").hide();
            //    $('#radioButtons').hide();
            //    $('#sendButton').hide();
            //    $("#resultLink").show();
            //    $('#UserName').hide();
            //    $('#lblEnterName').hide();
            //}
            self.gotoResult();
        });
    };

    this.gotoResult = function() {
        var url = "/Result/Index/" + getMeetingId();
        window.location.assign(url);
    };
};