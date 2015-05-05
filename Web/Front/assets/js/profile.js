/// <reference path="../plugins/jquery/jquery-1.11.2.js" />
/// <reference path="../plugins/amcharts/amcharts.js" />
var handleProfilePieChart = function (inAmount,outAmount) {

    $("#assetsCharts").size() > 0 ? console.log(123) : console.log(3)
    var chart = AmCharts.makeChart("assetsCharts", {
        "type": "pie",
        "theme": "light",
        "dataProvider": [{
            "title": "收入",
            "value": inAmount
        }, {
            "title": "支出",
            "value": outAmount
        }],
        "titleField": "title",
        "valueField": "value",
        "labelRadius": 0,
        "startDuration": 0,
        "radius": "50%",
        "innerRadius": "40%",
        "labelText": "[[title]]",
        "export": {
            "enabled": true,
            "libs": {
                "path": "http://www.amcharts.com/lib/3/plugins/export/libs/"
            }
        }
    });
}

//    function () {
//    Highcharts.theme = {
//        colors: ["#7cb5ec", "#f7a35c", "#90ee7e", "#7798BF", "#aaeeee", "#ff0066", "#eeaaee",
//           "#55BF3B", "#DF5353", "#7798BF", "#aaeeee"],
//        chart: {
//            backgroundColor: null,
//            style: {
//                fontFamily: "Dosis, sans-serif"
//            }
//        },
//        title: {
//            style: {
//                fontSize: '16px',
//                fontWeight: 'bold',
//                textTransform: 'uppercase'
//            }
//        },
//        tooltip: {
//            borderWidth: 0,
//            backgroundColor: 'rgba(219,219,216,0.8)',
//            shadow: false
//        },
//        legend: {
//            itemStyle: {
//                fontWeight: 'bold',
//                fontSize: '13px'
//            }
//        },
//        xAxis: {
//            gridLineWidth: 1,
//            labels: {
//                style: {
//                    fontSize: '12px'
//                }
//            }
//        },
//        yAxis: {
//            minorTickInterval: 'auto',
//            title: {
//                style: {
//                    textTransform: 'uppercase'
//                }
//            },
//            labels: {
//                style: {
//                    fontSize: '12px'
//                }
//            }
//        },
//        plotOptions: {
//            candlestick: {
//                lineColor: '#404048'
//            }
//        },


//        // General
//        background2: '#F0F0EA'

//    };

//    // Apply the theme
//    Highcharts.setOptions(Highcharts.theme);
//    Highcharts.getOptions().colors = Highcharts.map(Highcharts.getOptions().colors, function (color) {
//        return {
//            radialGradient: { cx: 0.5, cy: 0.3, r: 0.7 },
//            stops: [
//                [0, color],
//                [1, Highcharts.Color(color).brighten(-0.3).get('rgb')] // darken
//            ]
//        };
//    });
//    $('#assetsCharts').highcharts({
//        chart: {
//            plotBackgroundColor: null,
//            plotBorderWidth: null,
//            plotShadow: false,
//            height: 180,
//            width: 240
//        },
//        title: {
//            text: 'Assets',
//            floating: true,
//            style: {display:"none"}
//        },
//        tooltip: {
//            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
//        }, 
//        plotOptions: {
//            pie: {
//                allowPointSelect: true,
//                cursor: 'pointer',
//                dataLabels: {
//                    enabled: true,
//                    distance: -10,
//                    format: '{point.name}: {point.percentage:.0f} %',
//                    style: {
//                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
//                    }  
//                }
//            } 
//        },
//        series: [{
//            type: 'pie',
//            name: 'Assets',
//            data: [
//                ['CNY', 25.0],
//                ['USD', 26.8],
//                ['XRP', 8.5]
//            ]
//        }]
//    });
//};

var Profile = function () {
    "use strict";
    return {
        handleChart: function (inAmount,outAmount) {
            $.getScript('/assets/plugins/amcharts/pie.js').done(function () {
                $.getScript('/assets/plugins/amcharts/themes/light.js').done(function () {
                    handleProfilePieChart(inAmount, outAmount);
                });
            });
        }
    };
}();