import Image from 'react-bootstrap/Image';
import Carousel from 'react-bootstrap/Carousel';
import Modal from 'react-bootstrap/Modal';
import { useUI } from '../../context/UIContext';
import { usePoint } from '../../context/PointContext';
import API_URL from '../../services/apiConfig';

// Child component of PointShow

const ImageCarousel = () => {
    const { showImageCarousel, setShowImageCarousel, setShowPoint } = useUI();
    const { selectedPoint } = usePoint();
    const handleCloseClick = () => {
        setShowImageCarousel(false);
        setShowPoint(true);
    }
    return (
        <Modal show={showImageCarousel} onHide={handleCloseClick} size="xl" >
            <Carousel>
                {selectedPoint?.images.map(image =>
                    <Carousel.Item key={image.imageId} style={{height: '60vh'}}>
                        <Image
                            src={`${API_URL}/${image.filePath}`}
                            style={{objectFit: 'contain'}}
                            className="w-100 h-100" />
                    </Carousel.Item>
                    
                )}
            </Carousel>
        </Modal>
    );
};

export default ImageCarousel;