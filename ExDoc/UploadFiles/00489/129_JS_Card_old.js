
        $(function () {
            var querystring = location.search.replace('?', '').split('&');
            var queryObj = {};
            // loop through each name-value pair and populate object
            for (var i = 0; i < querystring.length; i++) {
                // get name and value
                var name = querystring[i].split('=')[0];
                var value = querystring[i].split('=')[1];
                // populate object
                queryObj[name] = decodeURIComponent(value);
            }
            
            $("#nameth").text(queryObj["nameth"]);
            $("#namejap").text(queryObj["namejap"]);
            $("#nameeng").text(queryObj["nameeng"]);
            $("#group").text(queryObj["group"]);
            $("#position").text(queryObj["position"]);
            $("#phone").text(queryObj["phone"].substring(0, 4) + "-" + queryObj["phone"].substring(4));
            $("#ext").text(queryObj["ext"]);
            $("#fax").text(queryObj["fax"].substring(0, 4) + "-" + queryObj["fax"].substring(4));
            $("#mobile").text(queryObj["mobile"].substring(0, 4) + "-" + queryObj["mobile"].substring(4));
            $("#email").text(queryObj["email"]);
            
            var plant = queryObj["plant"];
            if (plant == "BPK") {
                $("#spnAddr1").text("700/452 Moo 7, Bangna-Trad");
                $("#spnAddr2").text("Tumbol Don Hua Roh,");
                $("#spnAddr3").text("Amphur Muang, Chonburi 20000");

                $("#spnTax").text("เลขประจำตัวผู้เสียภาษี 0105531081447");
                $("#spnSite").text("(สำนักงานใหญ่)");
            }
            else if (plant == "PTN") {
                $("#spnAddr1").text("700/806 Moo 1");
                $("#spnAddr2").text("Tumbol Panthong,");
                $("#spnAddr3").text("Amphur Muang, Chonburi 20160");

                $("#spnTax").text("เลขประจำตัวผู้เสียภาษี 0105531081447");
                $("#spnSite").text("(สาขา 3)");
            }
        });
    