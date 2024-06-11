import React, { useState } from 'react';
import { OverlayTrigger, Tooltip } from 'react-bootstrap';

import SubUserRow from './SubUserRow';
import { Cell, Row } from '@jsroot/components/react-grid/index';

const MainUserRow = ({ user, onAddSubUser, onDeleteSubUser }) => {
    const [isExpand, setExpand] = useState(true);

    const handleExpand = () => setExpand(!isExpand);

    const handleAddSubUser = () => onAddSubUser({ show: true, selectedUser: user });

    const handleDeleteSubUser = subUser => onDeleteSubUser(user, subUser);

    return (
        <>
            <Row className="main-user">
                <Cell className="text-center" onClick={handleExpand}>
                    <span className={`fa fa-chevron-${isExpand ? 'up' : 'down'}`} />
                </Cell>
                <Cell>{user.companyName}</Cell>
                <Cell>{user.code}</Cell>
                <Cell>{user.nickname}</Cell>
                <Cell>{user.fullName}</Cell>
                <Cell>{user.departmentName}</Cell>
                <Cell>{user.sectionName}</Cell>
                <Cell>{user.teamName}</Cell>
                {
                    window.hasHostPermission &&
                    <Cell className="text-center">
                        <OverlayTrigger
                            placement='auto'
                            overlay={
                                    <Tooltip id={`tooltip-add-sub-user-${user.userId}`}>
                                        Add sub-user
                                    </Tooltip>
                                }
                            >
                                <span className='rounded-circle fa fa-plus icon-create' onClick={handleAddSubUser} />
                            </OverlayTrigger>
                    </Cell>
                }
            </Row>
            {
                isExpand &&
                user.subUsers.map((subUser, index) =>
                    <SubUserRow key={`sub-user-${subUser.userId}`} subUser={subUser} index={index + 1} onDelete={handleDeleteSubUser} />)
            }
        </>
    );
};

export default MainUserRow;