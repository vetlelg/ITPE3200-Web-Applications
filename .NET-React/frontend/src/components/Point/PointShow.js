import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import Image from 'react-bootstrap/Image';
import Carousel from 'react-bootstrap/Carousel';
import { useUI } from '../../context/UIContext';
import { usePoint } from '../../context/PointContext';
import API_URL from '../../services/apiConfig';
import CommentList from './CommentList';
import { CardText, GeoAltFill, PersonBadge, StarFill } from 'react-bootstrap-icons';
import { useAccount } from '../../context/AccountContext';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';

const Point = () => {
    const { showPoint, setShowPoint, setShowImageCarousel, setShowPointEdit } = useUI();
    const { selectedPoint, deletePoint } = usePoint();
    const { accountId } = useAccount();

    const handleCloseClick = () => setShowPoint(false);

    const handleImageClick = () => {
        setShowImageCarousel(true);
    }

    const handleDeleteClick = async () => {
        if (window.confirm("Delete point? Point will be permanently deleted")) {
            deletePoint(selectedPoint.pointId);
            setShowPoint(false);
        }
    }

    const handleEditClick = () => {
        setShowPointEdit(true);
    }

    let showAverage;
    let totalRating = 0.0;
    let average = 0.0;
    if (selectedPoint && selectedPoint?.comments?.length > 0) {
        showAverage = true
        //Calculates average rating by comments
        selectedPoint.comments.map(comment =>
        (
            totalRating += comment.rating
        ))
        average = totalRating / selectedPoint.comments.length;
        average = average.toFixed(2)
    } else {
        showAverage = false
    }

    return (
        <Modal show={showPoint} onHide={handleCloseClick} centered size='xl' scrollable>
            <Modal.Header closeButton>
                <Modal.Title>{selectedPoint?.name}</Modal.Title>
            </Modal.Header>

            <Modal.Body className="d-flex justify-content-center w-100">
                <Row className="w-100">
                    
                    <Col lg={7} className="mb-2">
                    <Carousel interval={null} className='mb-3 bg-dark'>
                        {selectedPoint?.images.map(image =>
                            <Carousel.Item key={image.imageId} style={{ height: '30vh' }}>
                                
                                <Image
                                    src={`${API_URL}/${image.filePath}`}
                                    style={{ cursor: 'pointer', objectFit: 'contain', width: '100%', height: '100%' }}
                                    onClick={handleImageClick} />
                                    
                            </Carousel.Item>
                        )}
                        {selectedPoint?.images.length === 0 &&
                            <Carousel.Item  style={{ height: '30vh' }}><Carousel.Caption ><h4 className="mb-5 pb-4">No images</h4></Carousel.Caption></Carousel.Item>}
                    </Carousel>
                        <h4><CardText />  Description: {selectedPoint?.description}</h4>
                        <h4><GeoAltFill />  Position: ({selectedPoint?.latitude}, {selectedPoint?.longitude})</h4>
                        <h4><PersonBadge />  Account: {selectedPoint?.account.email} </h4>
                        {showAverage && <h4><StarFill />  Rating: {average} </h4>}
                        {selectedPoint?.accountId === accountId && accountId &&
                            <>
                                <Button key="1" onClick={handleEditClick} className="btn btn-primary m-1" > Edit</Button>
                                <Button key="2" onClick={() => handleDeleteClick(selectedPoint.pointId)} className="btn btn-danger">Delete</Button>
                            </>}
                    </Col>
                    <Col lg={5} style={{ maxHeight: '55vh', overflowY: 'auto' }}>
                        <CommentList />
                    </Col>
                </Row>
            </Modal.Body>


        </Modal>
    );
};

export default Point;