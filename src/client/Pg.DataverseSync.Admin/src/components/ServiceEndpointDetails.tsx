import { useGetEnvironmentVariableValue } from '../hooks/useGetEnvironmentVariableValue';

export function ServiceEndpointDetails() {
  const { value, loading } = useGetEnvironmentVariableValue('pg_dataversesyncendpointid');

  const label = loading
    ? 'Loading service endpoint details...'
    : (value ?? 'No Service Endpoint configured.');

  return <p>{label}</p>;
}
