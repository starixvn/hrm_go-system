import React, { useState, Fragment } from 'react';
import { Button } from 'react-bootstrap';

import Row from './Row';
import TotalRow from './TotalRow';
import SubTotalRow from './SubTotalRow';

import LangHelper from '@jsroot/common/langHelper';

const Grid = ({ employeeInfos, data, detail, onShowDetail, jobLevelTypes }) => {
    const {
        headers, supervisors, employees, others, totalSupervisor, totalEmployee, grandTotal,
        totalJobTypeSupervisor, totalJobTypeEmployee, totalJobTypeOthers
    } = data;

    const showHides = () => {
        const showHides = [];
        totalJobTypeSupervisor.map(item => {
            showHides.push({
                key: item.name,
                show: false
            })
        });

        totalJobTypeEmployee.map(item => {
            showHides.push({
                key: item.name,
                show: false
            })
        });

        showHides.push({
            key: 'Other',
            show: false
        });

        return showHides;
    };

    const [showHideOptions, setShowHideOptions] = useState(showHides);
    const [showMore, setShowMore] = useState(false);

    const handleShowHideCells = unit => {
        const options = [...showHideOptions];
        const index = options.findIndex(x => x.key === unit);
        options[index] = { ...options[index], show: !options[index].show };
        setShowHideOptions(options);
    };

    const handleShowDetail = (event, jobLevel, country, gunit) => {
        if (!detail.show) {
            const units = headers.find(item => item.name === country).cells.find(item => item.name === gunit).cells;
            const emps = employeeInfos.filter(emp => emp.jobLevel === jobLevel && emp.country === country && units.findIndex(unit => unit.name === emp.unitName) >= 0);
            onShowDetail(event.target, emps);
        }
    };

    const renderRows = (rows, jobLevelTypes, totalJobType, isOther = false) => {
        const trs = [];
        jobLevelTypes.map(jobType => {
            trs.push(<Row key={jobType}
                data={rows}
                jobType={jobType}
                handleShowHideCells={handleShowHideCells}
                totalJobType={totalJobType}
                showHideOptions={showHideOptions}
                onClick={handleShowDetail}
                isOther={isOther}/>);
        });

        return trs;
    };

    return (
        <>
            <table className='table group-by-country'>
                <thead>
                    <tr>
                        <td rowSpan={2} colSpan={2} className='col-job-level'>{LangHelper.getResource('JobLevel')}</td>
                        {
                            headers.map(country => <td key={country.name} rowSpan={country.name !== 'Other' ? 1 : 2} colSpan={country.name !== 'Other' ? (country.cells.length + 1) : 1}>{country.name}</td>)
                        }
                        <td rowSpan={2} className='col-job-level'>Grand Total</td>
                    </tr>
                    <tr>
                        {
                            headers.map(country => <Fragment key={country.name}>
                                {
                                    country.name !== 'Other' && <>
                                        {
                                            country.cells.map(company => 
                                                <td key={`header-company-${country.name}-${company.name}`} className='col-total'>{company.name}</td>
                                            )
                                        }
                                        <td key={`header-company-total-${country.name}`} className='col-total'>Total</td>
                                    </>
                                }
                            </Fragment>)
                        }
                    </tr>
                </thead>
                <tbody>
                    <SubTotalRow data={grandTotal} totalSupervisor={totalSupervisor} totalEmployee={totalEmployee} />
                    {
                        showMore &&
                        <>
                            {renderRows(supervisors, jobLevelTypes.Supervisor, totalJobTypeSupervisor)}
                            <TotalRow data={totalSupervisor} />
                            {renderRows(employees, jobLevelTypes.Employee, totalJobTypeEmployee)}
                            <TotalRow data={totalEmployee} />
                            {renderRows(others, jobLevelTypes.Other, totalJobTypeOthers, true)}
                        </>
                    }
                </tbody>
            </table>
            <Button className='btn-show-more' onClick={() => setShowMore(!showMore)}>Show {showMore ? 'less' : 'more'}</Button>
            <hr />
        </>
    );
};

export default Grid;