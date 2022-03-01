export interface Pokedex {
  type: string;
  geometry: Geometry;
  properties: Properties;
}

export interface Geometry {
  type: string;
  coordinates: Array<Array<number[]>>;
}

export interface Properties {
  name: string;
  description: string;
}
export const styleObject = {
  clickable: true,
  fillColor: 'rgba(143,75,75,0.92)',
  strokeWeight: 1,
};
