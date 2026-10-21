(function () {

    "use strict";


    // =========================================================
    // PAGE INITIALISATION
    // =========================================================

    document.addEventListener(
        "DOMContentLoaded",
        function () {

            drawPerformanceTrend();

        }
    );



    // =========================================================
    // SCORE PROGRESS TREND
    // =========================================================

    function drawPerformanceTrend() {

        // ---------------------------------------------------------
        // hfTrendJson is a hidden ASP.NET field.
        //
        // Performance.aspx.cs stores the student's monthly
        // score trend as JSON inside this hidden field.
        //
        // This JavaScript reads that JSON and draws the chart.
        // ---------------------------------------------------------

        const hidden =
            document.getElementById(
                "hfTrendJson"
            );


        const canvas =
            document.getElementById(
                "scoreTrendCanvas"
            );


        if (
            !hidden
            ||
            !canvas
        ) {
            return;
        }



        // =====================================================
        // READ JSON DATA
        // =====================================================

        let data = [];


        try {

            if (
                hidden.value
                &&
                hidden.value.trim() !== ""
            ) {
                data =
                    JSON.parse(
                        hidden.value
                    );
            }

        }
        catch (error) {

            console.error(
                "Unable to read performance trend data.",
                error
            );

            data = [];
        }



        // =====================================================
        // CANVAS SIZE
        // =====================================================

        const container =
            canvas.parentElement;


        const displayWidth =
            container.clientWidth;


        const displayHeight =
            290;


        canvas.width =
            displayWidth;


        canvas.height =
            displayHeight;



        const ctx =
            canvas.getContext(
                "2d"
            );


        ctx.clearRect(
            0,
            0,
            displayWidth,
            displayHeight
        );



        // =====================================================
        // NO DATA
        // =====================================================

        if (
            !Array.isArray(data)
            ||
            data.length === 0
        ) {

            drawNoDataMessage(
                ctx,
                displayWidth,
                displayHeight
            );

            return;
        }



        // =====================================================
        // CHART SETTINGS
        // =====================================================

        const padding = {

            top: 32,

            right: 30,

            bottom: 52,

            left: 52

        };


        const chartWidth =
            displayWidth
            - padding.left
            - padding.right;


        const chartHeight =
            displayHeight
            - padding.top
            - padding.bottom;



        // =====================================================
        // DRAW GRID
        // =====================================================

        drawGrid(
            ctx,
            displayWidth,
            chartHeight,
            padding
        );



        // =====================================================
        // CREATE POINT POSITIONS
        // =====================================================

        const points =
            createPoints(
                data,
                chartWidth,
                chartHeight,
                padding
            );



        // =====================================================
        // DRAW TREND
        // =====================================================

        drawTrendLine(
            ctx,
            points
        );


        drawTrendPoints(
            ctx,
            points,
            displayHeight
        );

    }



    // =========================================================
    // NO DATA MESSAGE
    // =========================================================

    function drawNoDataMessage(
        ctx,
        width,
        height
    ) {

        ctx.font =
            "14px Segoe UI";


        ctx.fillStyle =
            "#77718c";


        ctx.textAlign =
            "center";


        ctx.textBaseline =
            "middle";


        ctx.fillText(
            "No graded performance data available.",
            width / 2,
            height / 2
        );

    }



    // =========================================================
    // GRID AND Y AXIS
    // =========================================================

    function drawGrid(
        ctx,
        width,
        chartHeight,
        padding
    ) {

        ctx.font =
            "12px Segoe UI";


        ctx.textAlign =
            "right";


        ctx.textBaseline =
            "middle";


        for (
            let value = 0;
            value <= 100;
            value += 25
        ) {

            const y =
                padding.top
                +
                chartHeight
                -
                (
                    value / 100
                )
                *
                chartHeight;



            // GRID LINE

            ctx.beginPath();


            ctx.strokeStyle =
                "#ece9f3";


            ctx.lineWidth = 1;


            ctx.moveTo(
                padding.left,
                y
            );


            ctx.lineTo(
                width - padding.right,
                y
            );


            ctx.stroke();



            // Y AXIS LABEL

            ctx.fillStyle =
                "#77718c";


            ctx.fillText(
                value + "%",
                padding.left - 10,
                y
            );

        }

    }



    // =========================================================
    // CREATE CHART POINTS
    // =========================================================

    function createPoints(
        data,
        chartWidth,
        chartHeight,
        padding
    ) {

        const points = [];


        for (
            let i = 0;
            i < data.length;
            i++
        ) {

            let x;


            if (
                data.length === 1
            ) {

                x =
                    padding.left
                    +
                    chartWidth / 2;

            }
            else {

                x =
                    padding.left
                    +
                    (
                        i /
                        (
                            data.length - 1
                        )
                    )
                    *
                    chartWidth;

            }



            let score =
                parseFloat(
                    data[i].Score
                );


            if (
                Number.isNaN(score)
            ) {
                score = 0;
            }



            // Keep percentage within chart range.

            score =
                Math.max(
                    0,
                    Math.min(
                        100,
                        score
                    )
                );



            const y =
                padding.top
                +
                chartHeight
                -
                (
                    score / 100
                )
                *
                chartHeight;



            points.push({

                x: x,

                y: y,

                score: score,

                label:
                    data[i].Label || ""

            });

        }


        return points;

    }



    // =========================================================
    // DRAW PURPLE TREND LINE
    // =========================================================

    function drawTrendLine(
        ctx,
        points
    ) {

        if (
            points.length === 0
        ) {
            return;
        }


        ctx.beginPath();


        ctx.strokeStyle =
            "#7c28ef";


        ctx.lineWidth = 3;


        ctx.lineJoin =
            "round";


        ctx.lineCap =
            "round";


        for (
            let i = 0;
            i < points.length;
            i++
        ) {

            const point =
                points[i];


            if (
                i === 0
            ) {

                ctx.moveTo(
                    point.x,
                    point.y
                );

            }
            else {

                ctx.lineTo(
                    point.x,
                    point.y
                );

            }

        }


        ctx.stroke();

    }



    // =========================================================
    // DRAW POINTS, SCORES AND MONTH LABELS
    // =========================================================

    function drawTrendPoints(
        ctx,
        points,
        canvasHeight
    ) {

        // Prevent too many month labels overlapping
        // when All Time contains many months.

        const labelStep =
            Math.max(
                1,
                Math.ceil(
                    points.length / 8
                )
            );


        for (
            let i = 0;
            i < points.length;
            i++
        ) {

            const point =
                points[i];



            // -------------------------------------------------
            // POINT
            // -------------------------------------------------

            ctx.beginPath();


            ctx.fillStyle =
                "#7c28ef";


            ctx.arc(
                point.x,
                point.y,
                5,
                0,
                Math.PI * 2
            );


            ctx.fill();



            // -------------------------------------------------
            // SCORE
            // -------------------------------------------------

            ctx.font =
                "600 11px Segoe UI";


            ctx.fillStyle =
                "#292052";


            ctx.textAlign =
                "center";


            ctx.textBaseline =
                "bottom";


            ctx.fillText(
                point.score.toFixed(1)
                + "%",
                point.x,
                point.y - 9
            );



            // -------------------------------------------------
            // MONTH
            //
            // Show fewer labels if there are many months.
            // Always show the final month.
            // -------------------------------------------------

            const shouldShowLabel =
                i % labelStep === 0
                ||
                i === points.length - 1;


            if (
                shouldShowLabel
            ) {

                ctx.font =
                    "11px Segoe UI";


                ctx.fillStyle =
                    "#77718c";


                ctx.textBaseline =
                    "middle";


                ctx.fillText(
                    point.label,
                    point.x,
                    canvasHeight - 19
                );

            }

        }

    }



    // =========================================================
    // REDRAW CHART WHEN BROWSER SIZE CHANGES
    // =========================================================

    let resizeTimer;


    window.addEventListener(
        "resize",
        function () {

            clearTimeout(
                resizeTimer
            );


            resizeTimer =
                setTimeout(
                    function () {

                        drawPerformanceTrend();

                    },
                    120
                );

        }
    );

})();