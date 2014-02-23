function resultsType() {
    var self = this;

    this.buildChart = function (results, maxValue) {
        var ticks = [];
        var convenient = [];
        var inconvenient = [];
        var users = [];
        var userName;
        for (var index in results) {
            var result = results[index];
            ticks.push(result.Key);
            convenient.push(result.Convenient);
            inconvenient.push(result.Inconvenient);
            
            for (var userNameConvIndex in result.UserNamesConvenient) {
                userName = result.UserNamesConvenient[userNameConvIndex];
                if(!users[result.Key])
                    users[result.Key] = {userNamesConvenient: [], userNamesInconvenient: []};    
                users[result.Key].userNamesConvenient.push(userName);
            }
            
            for (var userNameInconvIndex in result.UserNamesInconvenient) {
                userName = result.UserNamesInconvenient[userNameInconvIndex];
                if (!users[result.Key])
                    users[result.Key] = { userNamesConvenient: [], userNamesInconvenient: [] };
                users[result.Key].userNamesInconvenient.push(userName);
            }
        }
        
        $.jqplot('chartResults', [convenient, inconvenient], {
            seriesColors: ['green', '#E96A6D'],
            // The "seriesDefaults" option is an options object that will
            // be applied to all series in the chart.
            seriesDefaults: {
                renderer: $.jqplot.BarRenderer,
                rendererOptions: { fillToZero: true }
            },
            labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
            legend: {
                show: false
            },
            axes: {
                // Use a category axis on the x axis and use our custom ticks.
                xaxis: {
                    renderer: $.jqplot.CategoryAxisRenderer,
                    ticks: ticks
                },
                //// Pad the y axis just a little so bars can get close to, but
                //// not touch, the grid boundaries.  1.2 is the default padding.
                yaxis: {
                    pad: 1,
                    labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                    tickInterval: 1,
                    min: 0,
                }
            },
                
            highlighter: {
                show: true,
                showMarker: false,
                showTooltip: true,
                tooltipContentEditor: function (str, seriesIndex, pointIndex, plot) {
                    var key = plot.axes.xaxis.ticks[pointIndex];
                    var objectNames = users[key];
                    var source;
                    if (seriesIndex == 0)
                        source = objectNames.userNamesConvenient;
                    else {
                        source = objectNames.userNamesInconvenient;
                    }
                    result = '';
                    var first = true;
                    for (index in source) {
                        if (!first)
                            result += ', ';
                        result += source[index];
                        first = false;
                    }
                    return result;
                }
            }
        });
    };
    
    this.SetAjaxInterval = function () {
        var interval = 1000 * 5; // where X is your every X minutes
        setInterval(self.makeResultQuery, interval);
    };

    this.makeResultQuery = function () {
        var meetingId = $('#meetingId').val();
        var rowVersion = $('#rowVersion').val();
        $.ajax({
            type: "POST",
            url: "/Result/GetResults/" + meetingId,
            data: {
                id: meetingId,
                currentRowVersion: rowVersion
            },
        }).done(function (model) {
            if (rowVersion != model.RowVersion) {
                $('#rowVersion').val(model.RowVersion);
                self.buildChart(model.Results, model.MaxValue);
                $('#message').html(model.Message);
            }
        });
    };
}

var results = null;
$(document).ready(function () {
    results = new resultsType();
    results.makeResultQuery();
    results.SetAjaxInterval();
});