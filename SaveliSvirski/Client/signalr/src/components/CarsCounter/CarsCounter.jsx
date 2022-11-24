import React from 'react';
import '../../Styles/App.css';

const CarsCounter = ({
    carsCount
}) => {
    
    return (
        <div className='CarsCounter'>
            <h1 id='carsCounterh1'>
                Available cars:
            </h1>
            <h2 id='counterValue'>{carsCount}</h2>
            
        </div>
    )
}

export default CarsCounter