const GetDealerListApiUrl = "/API/ContractListing/GetContractViewerModel?page={page}&filter={filter}&type={type}&contains={contains}&orderBy={orderBy}&asc={asc}";
const AddContractApiUrl = "/API/ContractListing/AddNewContract"; //post
const UploadContractApiUrl = "/API/ContractListing/UploadScan?dealerId={id}";
const GetScanPdfApiUrl = "/Scan/GetScan?scanId={id}";
const GetDocumentApiUrl = "/Document/GetDocument?dealerId={id}&docName={name}";
const CurrentHost = window.location.protocol + '//' + window.location.host;