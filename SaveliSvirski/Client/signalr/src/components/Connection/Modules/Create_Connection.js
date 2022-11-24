import * as SignalR from '@microsoft/signalr';
import * as Constants from '../../../Constants/Constants.js';
import * as CounterHandlers from './SetCarsCounterHandlers.js'

function CreateConnection(
    token,
    setCarsCount,
    setConnection) {

    const hubConnection = new SignalR.HubConnectionBuilder()
        .withUrl(process.env.REACT_APP_CATALOGHUB_URL, {
            skipNegotiation: true,
            transport: SignalR.HttpTransportType.WebSockets,
            accessTokenFactory: () => token
        })
        .withAutomaticReconnect()
        .build()

    hubConnection.start()
        .then(() => {
            hubConnection.invoke(Constants.GetInitCount);
            setConnection(c => c = hubConnection)
        })
        .catch(() => {
            console.log("Failed To Connect")
            setConnection(c => c = null)
        })


    hubConnection.on(Constants.CarRemovedNotify, () => {
        CounterHandlers.removeCar(setCarsCount)
        console.log('counter decremented')
    })

    hubConnection.on(Constants.CarAddedNotify, () => {
        CounterHandlers.addCar(setCarsCount)
        console.log('counter incremented')

    })

    hubConnection.on(Constants.ReceiveCount, (count) => {
        CounterHandlers.setInitCarsCount(setCarsCount, count)
        console.log('Receiving data from the hub - ' + count);
    })
}

export default CreateConnection;