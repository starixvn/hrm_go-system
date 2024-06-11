import React from 'react';

import { Cell, Row } from '@jsroot/components/react-grid/index';

const SubUserRow = ({ subUser, index, onDelete }) => {
    const handleDeleteSubUser = () => onDelete(subUser);

    return (
        <Row>
            <Cell className="text-center">{index}</Cell>
            <Cell>{subUser.companyName}</Cell>
            <Cell>{subUser.code}</Cell>
            <Cell>{subUser.nickname}</Cell>
            <Cell>{subUser.fullName}</Cell>
            <Cell>{subUser.departmentName}</Cell>
            <Cell>{subUser.sectionName}</Cell>
            <Cell>{subUser.teamName}</Cell>
            {
                window.hasHostPermission &&
                <Cell className="text-center">
                    <span className="rounded-circle fa fa-trash icon-delete"
                        onClick={handleDeleteSubUser}
                    />
                </Cell>
            }
        </Row>
    );
};

export default SubUserRow;