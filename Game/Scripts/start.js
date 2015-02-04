var map;
var myLatLng;
var markers = [];

function initialize() {
    var haightAshbury = new google.maps.LatLng(27.059125784374068, 14.4140625);
    var mapOptions = {
        zoom: 1,
        center: haightAshbury,
        mapTypeId: google.maps.MapTypeId.HYBRID,
        streetViewControl: false
    };
    map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);

    google.maps.event.addListener(map, 'click', function (event) {
        addMarker(event.latLng);
        var lat = event.latLng.lat();
        var lng = event.latLng.lng();

        myLatLng = new google.maps.LatLng(lat, lng);

        console.log("Latitude = " + lat);
        console.log("Longitude = " + lng);
        console.log("");

        $("#lat").val(lat);
        $("#lng").val(lng);
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

// ------------------------------------------------------ //

google.maps.event.addDomListener(window, 'load', initialize);
