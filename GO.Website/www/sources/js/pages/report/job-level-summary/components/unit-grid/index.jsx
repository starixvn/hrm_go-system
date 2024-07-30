import React, { useState } from 'react';
import Row from './Row';
import TotalRow from './TotalRow';
import GrandTotalRow from './GrandTotalRow';
import LangHelper from '@jsroot/common/langHelper';

import { renderNumber } from '../../common.funcs';

const Grid = ({ employeeInfos, data, detail, onShowDetail, jobLevelTypes }) => {
    const { headers, supervisors, employees, others, totalSupervisor, totalEmployee, totalOthers, grandTotal, totalJobTypeSupervisor, totalJobTypeEmployee, totalJobTypeOthers } = data;

    const showHideJobs = () => {
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
            key: "Other",
            show: false
        });
        return showHides;
    }

    const showHides = headers.map(gunit => {
        return {
            key: gunit.name,
            show: false
        };
    });
    const [showHideOptions, setShowHideOptions] = useState(showHides);
    const [showHideJobOptions, setShowHideJobOptions] = useState(showHideJobs);

    const handleShowHideCells = unit => {
        const options = [...showHideOptions];
        const index = options.findIndex(x => x.key === unit);
        options[index] = { ...options[index], show: !options[index].show };
        setShowHideOptions(options);
    };

    const handleShowHideJobCells = job => {
        const options = [...showHideJobOptions];
        const index = options.findIndex(x => x.key === job);
        options[index] = { ...options[index], show: !options[index].show };
        setShowHideJobOptions(options);
    };

    const handleShowDetail = (event, jobLevel, unitName, isLocal) => {
        if (!detail.show) {
            const emps = employeeInfos.filter(emp => emp.jobLevel === jobLevel && emp.unitName === unitName && emp.isLocal === isLocal);
            onShowDetail(event.target, emps);
        }
    };

    const renderRows = (rows, jobLevelTypes, totalJobType, isOther = false) => {
        const trs = [];
        jobLevelTypes.map(jobType => {
            trs.push(<Row key={jobType}
                data={rows}
                jobType={jobType}
                totalJobType={totalJobType}
                showHideOptions={showHideOptions}
                showHideJobOptions={showHideJobOptions}
                handleShowHideJobCells={handleShowHideJobCells}
                onClick={handleShowDetail}
                isOther={isOther}/>);
        });

        return trs;
    };

    const GetNameUnitTypeTotal = name => {
        const words = name.split(' ');
        let nameTotal = '';
        words.map(x => {
            nameTotal += x.substring(0, 1);
        })
        return words.length === 1 ? name : nameTotal;
    };

    const renderGrandTotal = data => {
        let grandTotal = 0;
        let colSpanGrandTotal = 1;

        data.cells.map(gunit => {
            gunit.cells.map(unit => {
                grandTotal += unit.totalLocal + unit.totalExpat;
            });
            colSpanGrandTotal += 4;
        })

        const demo = <tr className="row-total">
            <td className="text-center" rowSpan={2} colSpan={2}>Grand Total</td>
            <td key={`grand-total-table-country`} className="text-center" colSpan={colSpanGrandTotal}>{renderNumber(grandTotal)}</td>
        </tr>;
        return demo;
    }

    return (
        <table className="table group-by-country">
            <thead>
                <tr>
                    <td rowSpan={3} colSpan={2} className="col-job-level">{LangHelper.getResource('JobLevel')}</td>
                    {
                        headers.map(gunit => {
                            const showHideOption = showHideOptions.find(x => x.key === gunit.name);
                            const colSpan = (showHideOption.show ? gunit.cells.length * 2 : 0) + 2;

                            return (
                                <td key={gunit.name} className="group-unit" colSpan={colSpan}>
                                    <span>{gunit.name}</span>
                                    {
                                        gunit.name !== "Other" && 
                                        <span
                                            className={`fa ${showHideOption.show ? 'fa-minus-square-o' : 'fa-plus-square-o'}`}
                                            onClick={() => handleShowHideCells(gunit.name)}
                                        />
                                    }
                                </td>
                            );
                        })
                    }
                </tr>
                <tr>
                    {
                        headers.map(gunit => {
                            const cols = [];
                            const showHideOption = showHideOptions.find(x => x.key === gunit.name);
                            if (showHideOption.show) {
                                gunit.cells.map(unit => {
                                    cols.push(<td key={`${gunit.name}-${unit.name}`} colSpan={2}>{unit.name}</td>);
                                });
                            }

                            cols.push(<td key={`${gunit.name}-total`} colSpan={2}>{`${GetNameUnitTypeTotal(gunit.name)} Total`}</td>);

                            return cols;
                        })
                    }
                </tr>
                <tr>
                    {
                        headers.map(gunit => {
                            const cols = [];
                            const showHideOption = showHideOptions.find(x => x.key === gunit.name);
                            if (showHideOption.show) {
                                gunit.cells.map(unit => {
                                    cols.push(<td key={`${gunit.name}-${unit.name}-expat`}>Expat</td>);
                                    cols.push(<td key={`${gunit.name}-${unit.name}-local`}>Local</td>);
                                });
                            }

                            cols.push(<td key={`${gunit.name}-total-expat`}>Expat</td>);
                            cols.push(<td key={`${gunit.name}-total-local`}>Local</td>);

                            return cols;
                        })
                    }
                </tr>
            </thead>
            <tbody>
                <GrandTotalRow data={grandTotal} showHideOptions={showHideOptions} />
                {renderRows(supervisors, jobLevelTypes.Supervisor, totalJobTypeSupervisor)}
                <TotalRow data={totalSupervisor} showHideOptions={showHideOptions} />
                {renderRows(employees, jobLevelTypes.Employee, totalJobTypeEmployee)}
                <TotalRow data={totalEmployee} showHideOptions={showHideOptions} />
                {renderRows(others, jobLevelTypes.Other, totalJobTypeOthers, true)}
                {renderGrandTotal(grandTotal)}
            </tbody>
        </table>
    );
};

export default Grid;