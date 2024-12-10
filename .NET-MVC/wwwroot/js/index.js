// Object to store view-data coming from the server
// This is imported in the scripts-tag in index.cshtml on the server-side
export const IndexViewData = {
    pointsJSON: '',
};


// Used to store the Google Maps map object
let map;

// Gets the saved position and zoom from session storage
const savedPosition = JSON.parse(sessionStorage.getItem("mapPosition"));
const savedZoom = parseInt(sessionStorage.getItem("mapZoom"));

  // Define boundaries for the map
  const allowedBounds = {
    north: 85,   // North Pole
    south: -85,  // South Pole
    west: -180,  // Western longitude
    east: 180    // Eastern longitude
  };

// Custom map options
// Map styles are set in the Google Cloud Platform
let mapOptions = {
    // If saved position and zoom exists, use them. Otherwise, use default values
    center: savedPosition ? savedPosition : { lat: 59.9125, lng: 10.7521 },
    zoom: savedZoom ? savedZoom : 10,
    minZoom: 2,
    mapTypeControl: false, // Disable option to change map to satellite or terrain
    fullscreenControl: false, // Disable fullscreen option
    streetViewControl: false, // Disable street view option
    mapId: "b733c2ffaff0d5fb", // Map ID from Google Cloud Platform. Necessary for custom map styles
    restriction: {
        latLngBounds: allowedBounds,
        strictBounds: true
    }
};

// Initialize the map
async function initializeMap() {
    // Import the Map class from the Google Maps library
    const { Map } = await google.maps.importLibrary("maps");

    // Create the map with the map options
    map = new Map(document.getElementById("map"), mapOptions);

    // Populate the map with markers/points of interest
    await addMarkersOnMap();

    // When the map is clicked, create a point of interest
    // event is a click event from Maps API. latLng is the coordinates of the click event
    map.addListener("click", event => createPoint(event.latLng));

    // Saves the map position and zoom to session storage
    // when the map is moved or zoomed
    map.addListener("center_changed", () => saveMapPosition());
    map.addListener("zoom_changed", () => saveMapZoom());
    
}


// Populates the map with markers from the server
let points = [];
let objectPointList = [];
let filteredMarkers = [];
async function addMarkersOnMap() {
    
    // Parse JSON string to array of points
    const pointsList = JSON.parse(IndexViewData.pointsJSON);
    // Import the Marker class from the Google Maps library
    const { AdvancedMarkerElement } = await google.maps.importLibrary("marker");

    // Create Google Maps markers on the map for each point in array
    // and add a click event to each marker
    pointsList.forEach(point => {
        const position = { lat: point.latitude, lng: point.longitude };
        const marker = new AdvancedMarkerElement({ position: position, map: map });
        marker.addListener("click", () => showPoint(point.pointId));

        //Complete list of all markers
        points.push(marker);
    });

    objectPointList = pointsList;
}

// Saves marker position and redirects to the create point page
function createPoint(position) {
    // Converts position to json string
    position = JSON.stringify(position.toJSON());
    // Saves the position to session storage
    sessionStorage.setItem("markerPosition", position);
    // Redirects to the create point page
    window.location.href = '/Point/Create';
}

// Saves the map position to session storage
function saveMapPosition() {
    // Gets the center of the map and converts it to a json string
    const position = JSON.stringify(map.getCenter().toJSON());
    sessionStorage.setItem("mapPosition", position);
}

// Saves the map zoom to session storage
function saveMapZoom() {
    const zoom = map.getZoom();
    sessionStorage.setItem("mapZoom", zoom);
}

// Redirects to the show point page
function showPoint(id) {
    window.location.href = `Point/Show/${id}`;
}


// Filter points shown on map
export async function filterPoints(accountIdList, markerType) {

    // If the user only has selected accounts then it filters based on selected account Ids
    if (!markerType) {
        for (var i = 0; i < accountIdList.length; i++) {
            for (let j = 0; j < points.length; j++) {
                if (objectPointList[j].account.email == accountIdList[i]) {
                    filteredMarkers.push(points[j])
                }
            }
        }
    }
    // If the user only has selected markers to filter
    else if (!accountIdList) {
        for (let j = 0; j < points.length; j++) {
            if (objectPointList[j].type == markerType) {
                filteredMarkers.push(points[j])
            }
        }
    }
        // If the user selects to filter on both
    else {
        for (var i = 0; i < accountIdList.length; i++) {
            for (let j = 0; j < points.length; j++) {
                if (objectPointListt[j].account.email == accountIdList[i] && objectPointList[j].type == markerType) {
                    filteredMarkers.push(points[j])
                }
            }
        }
    }
    
    //Removes points from map
    for (let i = 0; i < points.length; i++) {
        points[i].setMap(null);
    }
    // Adds correct points to map
    for (let i = 0; i < filteredMarkers.length; i++) {
        filteredMarkers[i].setMap(map);
    }
    filteredMarkers = [];
}
//Resets points on map
export async function resetPoints() {

    for (let i = 0; i < points.length; i++) {
        points[i].setMap(map);
    }
}


// Runs when the page is loaded
initializeMap();