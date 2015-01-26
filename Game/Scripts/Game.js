

$(function () {

    var ajaxFormSubmit = function () {
        var $form = $(this);

        var options = {
            url: $form.attr("action"),
            type: $form.attr("method"),
            data: $form.serialize()
        };

        $.ajax(options).done(function (data) {
            var $target = $($form.attr("data-user-Target"));
            var $newHtml = $(data);
            $target.replaceWith($newHtml);
            $newHtml.effect("highlight");
        });
        return false;
    };
    var submitAutocompleteForm = function (event, ui) {
        var $input = $(this);
        $input.val(ui.item.label);

        var $form = $input.parents("form:first");
        $form.submit();
    };
    var createAutocomplete = function () {
        var $input = $(this);

        var options = {
            source: $input.attr("data-user-autocomplete"),
            select: submitAutocompleteForm
        };

        $input.autocomplete(options);
    };

    var getPage = function () {
        var $a = $(this);

        var options = {
            url: $a.attr("href"),
            data: $("form").serialize(),
            type: "get"
        };
        $.ajax(options).done(function (data) {
            var target = $a.parents("div.pagedList").attr("data-user-target");
            $(target).replaceWith(data);
        });
        return false;
    };
   
    var map;
    var myLatLng;
    var markers = [];

    function initialize() {
        var haightAshbury = new google.maps.LatLng(37.7699298, -122.4469157);
        var mapOptions = {
            zoom: 12,
            center: haightAshbury,
            mapTypeId: google.maps.MapTypeId.HYBRID
        };
        map = new google.maps.Map(document.getElementById('map-canvas'),
            mapOptions);

        google.maps.event.addListener(map, 'click', function (event) {
            addMarker(event.latLng);
            var lat = event.latLng.lat();
            var lng = event.latLng.lng();

            myLatLng = new google.maps.LatLng(lat, lng);

            console.log("Latitude = " + lat);
            console.log("Longitude = " + lng);
            console.log("");
        });

    }
    function addMarker(location) {
        deleteMarkers();
        var marker = new google.maps.Marker({
            position: location,
            map: map
        });
        markers.push(marker);
    }

    function setAllMap(map) {
        for (var i = 0; i < markers.length; i++) {
            markers[i].setMap(map);
        }
    }

    function deleteMarkers() {
        setAllMap(null);
        markers = [];
    }

    function calcDistance(p1, p2) {
        return (google.maps.geometry.spherical.computeDistanceBetween(p1, p2) / 1000).toFixed(2);
    }

    // ------------------------------------------------------ //

    google.maps.event.addDomListener(window, 'load', initialize);

    $("form[data-User-Ajax='true']").submit(ajaxFormSubmit);

    $("input[data-User-autocomplete]").each(createAutocomplete);

    $(".main-content").on("click", ".pagedList a", getPage);

});