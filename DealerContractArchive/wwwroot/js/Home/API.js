const GetContractsListApiUrl = "/API/ContractListing/GetContractViewerModel?page={page}&filter={filter}&type={type}&contains={contains}";
const AddContractApiUrl = "/API/ContractListing/AddNewContract";
const UploadContractApiUrl = "/API/ContractListing/UploadScan";
const GetScanPdfApiUrl = "/Scan/GetScan?contractId={id}";
const GetDocumentApiUrl = "/Document/GetDocument?contractid={id}&docName={name}";
const CurrentHost = window.location.protocol + '//' + window.location.host;