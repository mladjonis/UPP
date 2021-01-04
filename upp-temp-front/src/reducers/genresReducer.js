import { GET_ALL_GENRES } from "../actions/types";

const initState = [];

const genresReducer = (state = initState, action) => {
  switch (action.type) {
    case GET_ALL_GENRES:
      return [...state, action.payload];
    default:
      return state;
  }
};

export default genresReducer;
