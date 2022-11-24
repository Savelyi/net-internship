import React from "react"


const FailedToConnect = ({ connection }) => {

    return (
        <div className='ConnectionFailed'>
            {connection === null ? (
                <h2 id='ErrorMessage' style={{ color: 'red' }}>Failed To Connect</h2>
            ) : (
                <br />
            )
            }
        </div>
    )
}

export default FailedToConnect