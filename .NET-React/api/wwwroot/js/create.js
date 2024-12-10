// Getting the position of the marker from session storage
// and setting the position on the input fields for sending the position to the server
const json = sessionStorage.getItem("markerPosition");
const position = JSON.parse(json);
document.getElementById("latitude").value = position.lat;
document.getElementById("longitude").value = position.lng;