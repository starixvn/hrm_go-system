import React, { useEffect, useState } from 'react';
import { Button, Form } from 'react-bootstrap';
import LangHelper from '@jsroot/common/langHelper';
import dayjs from 'dayjs';
import Site from '@jsroot/common/site';
import DataService from '@jsroot/common/dataService';

const AppContainer = () => {
    const [month, setMonth] = useState(null);
    useEffect(() => {
        const now = dayjs();
        const month = now.month() + 1;
        setMonth(`${now.year()}-${month < 10 ? '0' :''}${month}`);
    })
    const handleSubmit = () => {

    }

    const handleExportExcel = async (e) => {
        e.preventDefault();
        const params = {};
        Site.blockUI();
        const url = Site.resolveClientUrl('GroupManpowerReport/ExportExcel');
        await DataService.getFileAsync(url, params).then(response => {
            const fileName = response.headers['content-disposition'].split('filename=')[1].split(';')[0];
            Site.downloadFile(response.data, fileName);
        }).then(() => Site.unblockUI());
    }

    const handleChangeMonth = (val) => {
        console.log(val);
        setMonth(val);
    }
    return (
        <div className='group-manpower-report'>
            <h1>Group Manpower Report</h1>
            <Form className="form-filter" onSubmit={handleSubmit}>
                <Form.Row bsPrefix="form-row">
                    <Form.Group>
                        <Form.Label>{LangHelper.getResource('Month')}</Form.Label>
                        <Form.Control
                            type="month"
                            value={month}
                            onChange={(e) => handleChangeMonth(e.target.value)}
                            className="border p-2 rounded-md"
                        />
                    </Form.Group>
                    
                    <Form.Group className="form-group-width">
                        {/* <Button variant="primary" size="sm" className="mr-3" type="submit" onClick={handleSubmit}>
                            {LangHelper.getResource('Submit')}
                        </Button> */}
                        <Button variant="primary" size="sm" className="mr-3" type="submit" onClick={handleExportExcel}>
                            {LangHelper.getResource('ExportExcel')}
                        </Button>
                    </Form.Group>
                </Form.Row>

                 
            </Form>
        </div>
    );
}

export default AppContainer;