import React, { useState } from 'react';
import CreateConnection from './Modules/Create_Connection.js';
import SetTokenHandler from './Modules/SetTokenHandler.js';
import FailedToConnect from './FailedToConnect';


const Connection = ({
    setCarsCount,
    setConnection,
    connection
}) => {

    const [token, setToken] = useState('');

    return (
        <div className='Connection'>
            <h1 id='setTokenh1'>Set Token:</h1><br />
            <input
                id="tokenField"
                type="text"
                onChange={e => {
                    SetTokenHandler(setToken, e);
                }} /><br /><br />
            <button
                id="setTokenButton"
                onClick={e => {
                    CreateConnection(token, setCarsCount, setConnection)
                }
                }>Submit</button><br /><br />
            <FailedToConnect
                id='ConnectionFailed'
                connection={connection}
            />
        </div>

    )

}

export default Connection
