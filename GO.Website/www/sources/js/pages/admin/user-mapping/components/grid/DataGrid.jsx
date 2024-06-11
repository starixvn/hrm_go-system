import React from 'react';

import MainUserRow from './MainUserRow';
import { Cell, HeaderCell, HeaderRow, Row, Table } from '@jsroot/components/react-grid/index';

import LangHelper from '@jsroot/common/langHelper';

const DataGrid = ({ users, onAddSubUser, onDeleteSubUser }) => {
    const buildHeader = () => {
        return (
            <HeaderRow>
                <HeaderCell colWidth={5}>{LangHelper.getResource('SymbolNo')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Company')}</HeaderCell>
                <HeaderCell colWidth={8}>{LangHelper.getResource('Code')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Nickname')}</HeaderCell>
                <HeaderCell colWidth={14}>{LangHelper.getResource('FullName')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Department')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Section')}</HeaderCell>
                <HeaderCell colWidth={10}>{LangHelper.getResource('Team')}</HeaderCell>
                <HeaderCell colWidth={6}>{LangHelper.getResource('Action')}</HeaderCell>
            </HeaderRow>
        );
    };

    const buildBody = () => {
        if (users.length > 0) {
            return users.map(user =>
                <MainUserRow key={`main-user-${user.userId}`} user={user} onAddSubUser={onAddSubUser} onDeleteSubUser={onDeleteSubUser} />);
        }

        return [
            <Row key={'body'} className="no-data">
                <Cell colSpan={10}>{LangHelper.getResource('ThereIsNoData')}</Cell>
            </Row>
        ];
    };

    return (
        <Table
            minWidth={720}
            maxWidth={document.getElementById('app-container').clientWidth}
            autoWidth
            header={buildHeader()}
            body={buildBody()}
            containerPadding={0}
            adjustedHeight={10}
        />
    );
};

export default DataGrid;