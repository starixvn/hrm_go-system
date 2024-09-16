import React from 'react';

import { renderNumber } from '../../common.funcs';

const SubTotalRow = ({ data, totalSupervisor, totalEmployee }) => {
    return (
        <>
            <tr className='row-total'>
                <td className='text-center' rowSpan={2} colSpan={2}>{data.name}</td>
                {
                    totalSupervisor.cells.map((country, indexSup) => {
                        const cells = [];
                        if (country.name !== 'Other') {
                            country.cells.map((unit, indexUnit) => {
                                const totalUnitEmployee = totalEmployee.cells[indexSup].cells[indexUnit].total;
                                cells.push(<td key={`total-super-${country.name}-${unit.name}`} className='text-center'>{renderNumber(unit.total + totalUnitEmployee)}</td>);
                            });
                        } else {
                            let totalUnitEmployee = 0;
                            country.cells.map((unit, indexUnit) => {
                                totalUnitEmployee = unit.total + totalEmployee.cells[indexSup].cells[indexUnit].total;
                            });
                            cells.push(<td key={`total-super-${country.name}`} className='text-center'>{renderNumber(totalUnitEmployee)}</td>);
                        }

                        return cells;
                    })
                }
            </tr>
            <tr className='row-total'>
                {
                    data.cells.map(country =>
                        <td key={`sub-total-${country.name}`} className='text-center' colSpan={country.name === 'Other' ? 1 :4}>{renderNumber(country.cells[0].total)}</td>
                    )
                }
            </tr>
        </>
    );
};

export default SubTotalRow;