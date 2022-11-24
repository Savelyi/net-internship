import React, { useState } from "react";
import CarsCounter from "../CarsCounter/CarsCounter";
import Connection from "../Connection/Connection";

const BaseHandler = () => {

    const [carsCount, setCarsCount] = useState([]);
    const [connection, setConnection] = useState();

    return (
        <div className="BaseHandler">
            {(connection) ? (
                <CarsCounter
                    carsCount={carsCount}
                />
            ) : (
                <Connection
                    connection={connection}
                    setConnection={setConnection}
                    setCarsCount={setCarsCount}
                />
            )}
        </div>
    );
}

export default BaseHandler