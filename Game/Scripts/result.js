

var map;
var myLatLng;
var markers = [];

function initialize() {
     
    var mapOptions = {
        zoom: 5,
        center: new google.maps.LatLng(27.059125784374068, 14.4140625),
        mapTypeId: google.maps.MapTypeId.HYBRID,
        streetViewControl: false
    };
    map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);

    linepath = [
        new google.maps.LatLng(Number($("#lat1").val().replace(",", ".")), Number($("#lon1").val().replace(",", "."))),
        new google.maps.LatLng(Number($("#lat2").val().replace(",", ".")), Number($("#lon2").val().replace(",", ".")))
    ];

    addMarker(linepath[0]);
    addMarker(linepath[1]);

    var line = new google.maps.Polyline({
        path: linepath,
        geodesic: true,
        strokeColor: '#FF0000',
        strokeOpacity: 1.0,
        strokeWeight: 2
    });
    line.setMap(map);

}
function addMarker(location) {
    var marker = new google.maps.Marker({
        position: location,
        map: map
    });
    markers.push(marker);
}


// ------------------------------------------------------ //

google.maps.event.addDomListener(window, 'load', initialize);
