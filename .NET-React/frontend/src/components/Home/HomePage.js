import MapContainer from '../Map/MapContainer'
import Sidebar from '../Sidebar/Sidebar'
import Navigationbar from '../Navigationbar/Navigationbar'
import Container from 'react-bootstrap/Container';
import PointCreate from '../Point/PointCreate';
import PointEdit from '../Point/PointEdit';
import Account from "../Account/Account";
import Login from "../Account/Login";
import Register from "../Account/Register";
import Point from "../Point/PointShow";
import './HomePage.css';
import ImageCarousel from '../Point/ImageCarousel';

const HomePage = () => {

    /** 
     * The components are conditionally rendered based on the state variables.
     * MapContainer holds the map.
     * The map is set to be position absolute and cover the whole window.
     * 
     * The other UI components are rendered conditionally on top of the map.
     * 
     * Bootstrap or CSS styling can't be set directly on custom components,
     * so it has to be set within the components itself.
     * 
     * The order of the components below matter. MapContainer has to be rendered first.
     * If not, the Map will render above the UI components.
    */
    return (
        <Container fluid className="vh-100 p-0 position-relative">
            <MapContainer />
            <Navigationbar />
            <PointCreate />
            <PointEdit/>
            <Login />
            <Register />
            <Account />
            <Sidebar/>
            <Point />
            <ImageCarousel />
        </Container>
    );
};

export default HomePage;