
export const addCar = (setCarsCount) => {
    setCarsCount(carsCount => carsCount + 1);
}

export const removeCar = (setCarsCount) => {
    setCarsCount(carsCount => carsCount - 1);
}

export const setInitCarsCount = (setCarsCount,count) => {
    setCarsCount(carsCount => carsCount = count)
}