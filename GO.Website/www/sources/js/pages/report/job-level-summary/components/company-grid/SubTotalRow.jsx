import React from 'react';

import { renderNumber } from '../../common.funcs';

const SubTotalRow = ({ data, totalSupervisor, totalEmployee }) => {
    const renderSubTotalCompany = () => {
        let cells = [];
        let grandTotal = 0;
        totalSupervisor.cells.map((country, indexSup) => {
            let totalSub = 0;
            if (country.name !== 'Other') {
                country.cells.map((unit, indexUnit) => {
                    const totalUnitEmployee = totalEmployee.cells[indexSup].cells[indexUnit].total;
                    totalSub += unit.total + totalUnitEmployee;
                    cells.push(<td key={`total-super-${country.name}-${unit.name}`} className='text-center'>{renderNumber(unit.total + totalUnitEmployee)}</td>);
                });
                cells.push(<td key={`total-super-${country.name}`} className='text-center'>{renderNumber(totalSub)}</td>);
            } else {
                let totalUnitEmployee = 0;
                country.cells.map((unit, indexUnit) => {
                    totalUnitEmployee = unit.total + totalEmployee.cells[indexSup].cells[indexUnit].total;
                });
                cells.push(<td key={`total-super-${country.name}`} className='text-center'>{renderNumber(totalUnitEmployee)}</td>);
                grandTotal += totalUnitEmployee;
            }
            grandTotal += totalSub;
        });
        cells.push(<td key={`grand-total-super-company`} className='text-center'>{renderNumber(grandTotal)}</td>);

        return cells;
    };

    return (
        <>
            <tr className='row-total'>
                <td className='text-center' colSpan={2}>{data.name}</td>
                {renderSubTotalCompany()}
            </tr>
        </>
    );
};

export default SubTotalRow;