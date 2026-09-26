import { api } from './client';
import type { Instrument } from '../types';

export const getInstruments = () => api<Instrument[]>('/api/instruments');
