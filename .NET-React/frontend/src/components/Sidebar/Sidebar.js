import PointList from './PointList'
import { Offcanvas, Form, FormControl, Dropdown } from 'react-bootstrap';
import { useEffect, useState } from 'react';
import { useUI } from '../../context/UIContext';
import { usePoint } from '../../context/PointContext';


const Sidebar = () => {
    const { showSidebar, setShowSidebar } = useUI();
    const { points, setFilteredPoints } = usePoint();

    const handleCloseClick = () => setShowSidebar(false);

    // Users that are selected in the filtering dropdown
    const [selectedUsers, setSelectedUsers] = useState([]);
    // Search query in the sidebar
    const [query, setQuery] = useState('');

    // Filter points whenever list of points, selected users or query changes
    useEffect(() => {
        const filtered = filterPoints(points, selectedUsers, query);
        setFilteredPoints(filtered);
    }, [points, query, selectedUsers, setFilteredPoints]);

    // Manages filtering while taking both checkboxes and query into account
    const filterPoints = (points, selectedUsers, query) => {
        let filtered = points;

        if (selectedUsers.length > 0) {
            filtered = filtered.filter((point) => selectedUsers.includes(point.account.email));
        }

        if (query) {
            filtered = filtered.filter((point) =>
                point.name.toLowerCase().includes(query.toLowerCase()) ||
                point.description.toLowerCase().includes(query.toLowerCase())
            );
        }
        return filtered;
    };

    // Handles changes in the checkbox-filtering in Sidebar 
    const handleCheckboxChange = (user) => {
        setSelectedUsers((prevSelectedUsers) => {
            const updatedUsers = prevSelectedUsers.includes(user)
                ? prevSelectedUsers.filter((name) => name !== user)
                : [...prevSelectedUsers, user];

            const newFilteredPoints = filterPoints(points, updatedUsers, query);
            setFilteredPoints(newFilteredPoints);

            return updatedUsers;
        });
    };

    const handleQueryChange = e => setQuery(e.target.value);

    // Usernames to be displayed in the dropdown menu
    const uniqueUsernames = Array.from(new Set(points.map(point => point.account.email)));
    return (
        <div>
            <Offcanvas show={showSidebar} onHide={handleCloseClick} backdrop={false}>
                <Offcanvas.Header closeButton>
                    <Offcanvas.Title><h4>Points</h4></Offcanvas.Title>
                </Offcanvas.Header>
                <Offcanvas.Body>
                    <Form className="d-flex flex-column" onSubmit={e => e.preventDefault()}>
                        <FormControl
                            type="search"
                            placeholder="Search for points..."
                            className="mb-2"
                            aria-label="Search"
                            value={query}
                            onChange={e => handleQueryChange(e)}
                        />
                        <Dropdown autoClose="outside" className='mb-2'>
                            <Dropdown.Toggle variant='secondary' >Account</Dropdown.Toggle>
                            <Dropdown.Menu>
                                <div className='p-2'>
                                    {uniqueUsernames.map((email, index) => (
                                        <Form.Check
                                            key={index}
                                            type='checkbox'
                                            label={email}
                                            onChange={() => handleCheckboxChange(email)}
                                        />
                                    ))}
                                </div>
                            </Dropdown.Menu>
                        </Dropdown>
                    </Form>
                    <PointList />
                </Offcanvas.Body>
            </Offcanvas>
        </div>
    );
};


export default Sidebar;