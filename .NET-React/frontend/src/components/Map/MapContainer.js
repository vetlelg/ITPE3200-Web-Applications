import { APIProvider, Map, Marker } from "@vis.gl/react-google-maps";
import GOOGLE_MAPS_API_KEY from "./mapApiConfig";
import { darkMapStyle, lightMapStyle } from "./mapStyle";
import React from 'react';
import { useUI } from '../../context/UIContext';
import { useAccount } from "../../context/AccountContext";
import { usePoint } from "../../context/PointContext";

const MapContainer = () => {
    const { setShowLogin, setShowPoint, setShowPointCreate, isDarkTheme } = useUI();
    const { loggedIn } = useAccount();
    const { filteredPoints, setLatitude, setLongitude, setSelectedPoint } = usePoint();

    // Shows the point form for creating a new point when the map is clicked. Sets value of lat and long
    const handleMapClick = (event) => {
        if (loggedIn) {
            setLatitude(event.detail.latLng.lat);
            setLongitude(event.detail.latLng.lng)
            setShowPointCreate(true);
        } else {
            setShowLogin(true);
        }
    }

    // Shows the point details when a marker is clicked
    const handleMarkerClick = point => {
        setSelectedPoint(point);
        setShowPoint(true);
    }

    // Restricts the map to the bounds of the world
    const allowedBounds = {
        north: 85, // North pole
        south: -85, // South pole
        west: -180, // Western longitude
        east: 180 // Eastern longitude
    };

    const mapOptions = {
            disableDefaultUI: true,
            zoomControl: true,
            restriction: {
                latLngBounds: allowedBounds,
                strictBounds: true
            },
            styles: isDarkTheme ? darkMapStyle : lightMapStyle
            
    }

    // The markers are updated every time a point is added or removed from the list
    // which means that the markers on the map will be updated automatically
    return (
        <div className="position-absolute h-100 w-100 top-0">
        <APIProvider apiKey={GOOGLE_MAPS_API_KEY}>
            <Map
                key={isDarkTheme}
                onClick={e => handleMapClick(e)}
                options={mapOptions}
                defaultZoom={10}
                    defaultCenter={{ lat: 59.9125, lng: 10.7521 }} >
                {filteredPoints.map(point =>
                    <Marker
                        key={point.pointId}
                        position={{ lat: point.latitude, lng: point.longitude }}
                        onClick={() => handleMarkerClick(point)}
                    />
                )}

            </Map>
        </APIProvider>
        </div>
    );
};

export default MapContainer;