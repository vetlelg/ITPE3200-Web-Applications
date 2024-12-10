import Container from 'react-bootstrap/Container';
import { useUI } from '../../context/UIContext';
import { useAccount } from '../../context/AccountContext';
import { usePoint } from '../../context/PointContext';

// Child component of Sidebar

const PointList = () => {
    const { filteredPoints, setSelectedPoint, deletePoint } = usePoint();
    const { setShowPoint, setShowPointEdit } = useUI();
    const { accountId } = useAccount();

    const handleDeleteClick = (e, point) => {
        e.stopPropagation();
        if (window.confirm("Delete Point? Point will be permanently deleted"))
            deletePoint(point.pointId);
    };

    const handleEditClick = (e, point) => {
        e.stopPropagation();
        setSelectedPoint(point);
        setShowPointEdit(true);
    };

    const handlePointClick = point => {
        setSelectedPoint(point);
        setShowPoint(true);
    };

    return (
        <Container fluid className='p-0'>
            {filteredPoints.map(point => (
                <div onClick={() => handlePointClick(point)} key={point.pointId} className="border border-2 rounded-1 p-3 mb-2" style={{ cursor: 'pointer' }}>
                    <div className="d-flex justify-content-between align-items-center">
                        <h3 style={{ whiteSpace: 'normal', overflow: 'hidden', overflowWrap: 'break-word' }}>{point.name}</h3>
                        <div className="d-flex justify-content-end">
                            {point.accountId === accountId &&
                                <>
                                    <i
                                        onClick={e => handleEditClick(e, point)}
                                        key={`edit-${point.pointId}`}
                                        className="bi bi-pencil-square"
                                        style={{ cursor: 'pointer', fontSize: '1.3em' }}
                                    />
                                    <i
                                        onClick={e => handleDeleteClick(e, point)}
                                        key={`delete-${point.pointId}`}
                                        className="bi bi-trash-fill"
                                        style={{ cursor: 'pointer', fontSize: '1.3em' }}
                                    />
                                </>}
                        </div>
                    </div>
                    <p style={{ whiteSpace: 'normal', overflow: 'hidden', overflowWrap: 'break-word' }}>{point.description}</p>
                </div>
            ))}
        </Container>
    );
};

export default PointList;