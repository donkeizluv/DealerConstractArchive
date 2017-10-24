const GetDealerListApiUrl = "/API/ContractListing/GetContractViewerModel?page={page}&filter={filter}&type={type}&contains={contains}&orderBy={orderBy}&asc={asc}";
const AddContractApiUrl = "/API/ContractListing/AddNewContract"; //post
const UploadContractApiUrl = "/API/ContractListing/UploadScan?contractId={id}";
const GetScanPdfApiUrl = "/Scan/GetScan?contractId={id}";
const GetDocumentApiUrl = "/Document/GetDocument?contractid={id}&docName={name}";
const CurrentHost = window.location.protocol + '//' + window.location.host;