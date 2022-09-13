using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;using System.Text;



[ExecuteAlways]
public class StoryParser : Cycle
{
     private GoogleDriveFiles.ListRequest request;
     private GoogleDriveFiles.ExportRequest downloadRequest;
   public string filePath = string.Empty;
    public string result = string.Empty;
    public string resultText = string.Empty;
    private RangeInt range = new RangeInt();

    public string fileID = "1QVkVs9SnlgDg8RgEP7Ri-xjDOQtt8EDy97kBl_3Yesg";

    public override void Create()
    {   
        // only need to run this if we need a new fileID ( which will be hard coded above )!
        //StartCoroutine(GetFileByPathRoutine(filePath));
        downloadRequest = GoogleDriveFiles.Export(fileID, "text/plain");
        downloadRequest.Send().OnDone += SetResult;
        //print("start");
    }


   public List<List<List<string>>> fullText;
   

   private void SetResult (UnityGoogleDrive.Data.File file)
    {

        fullText = new List<List<List<string>>>();
        resultText = Encoding.UTF8.GetString(file.Content);
        //print(resultText);

        string[] separatingStrings = { "—---------" };
        string[] chapters = resultText.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

        for( int i = 0; i < chapters.Length; i+=2){

            string title = chapters[i+0];
            string content = chapters[i+1];


            List<List<string>> ChapterInfo = new List<List<string>>();

            separatingStrings[0] = "—-----";
            string[] stories = content.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

            for( int j = 1; j < stories.Length; j+=2){

                string storyTitle = stories[j+0];
                string storyContent = stories[j+1];

                storyTitle =  storyTitle.Trim('\r', '\n');

                List<string> StoryInfo = new List<string>();

                //print(storyTitle + ": Which Story :" + j );

                
                separatingStrings[0] = "—-";
                string[] pages = storyContent.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);

                //skipping first parse cuz its empty
                for( int k = 1; k < pages.Length; k+=1){


                    string page = pages[k]; 
                    page = page.Trim('\r', '\n');
                    
                    //print( " page num" + k );
                    //print(page);
                    StoryInfo.Add(page);

                }

                ChapterInfo.Add(StoryInfo);

            }

            fullText.Add(ChapterInfo);


        }




        PropogateToPages();


    }


    void PropogateToPages(){

        int chapterID = 0;
        int storyID = 0;
        int pageID = 0;
        foreach( StorySetter chapter in data.journey.setters){

            storyID = 0;
            foreach( Story story in chapter.stories ){

                pageID = 0;

                foreach( Page page  in story.pages ){

                    if( chapterID < fullText.Count  ){

                        if( storyID < fullText[chapterID].Count ){

                            if( pageID < fullText[chapterID][storyID].Count ){

                                //print("SOEMTHIGN IS RIGHT");
                                //print(fullText[chapterID][storyID][pageID]);
                                //print(page.gameObject.name);
                                page.text.text = fullText[chapterID][storyID][pageID];

                            }
                        }

                    }


                    pageID ++;

                }

                storyID ++;

            }

            chapterID ++;

        }


    }

   

















   /* 
   
   
    GETS NEW FILE LOCATION IF WE CHANGE IT!
   
   
   
   
   private IEnumerator GetFileByPathRoutine (string filePath)
    {
        // A folder in Google Drive is actually a file with the MIME type 'application/vnd.google-apps.folder'. 
        // Hierarchy relationship is implemented via File's 'Parents' property. To get the actual file using it's path 
        // we have to find ID of the file's parent folder, and for this we need IDs of all the folders in the chain. 
        // Thus, we need to traverse the entire hierarchy chain using List requests. 
        // More info about the Google Drive folders: https://developers.google.com/drive/v3/web/folder.

        var fileName = filePath;
        string parentNames = null;

        // Resolving folder IDs one by one to find ID of the file's parent folder.
        var parendId = "root"; // 'root' is alias ID for the root folder in Google Drive.
        if (parentNames != null)
        {
            for (int i = 0; i < parentNames.Length; i++)
            {
                request = new GoogleDriveFiles.ListRequest();
                request.Fields = new List<string> { "files(id)" };
                request.Q = $"'{parendId}' in parents and name = '{parentNames[i]}' and mimeType = 'application/vnd.google-apps.folder' and trashed = false";

                yield return request.Send();

                if (request.IsError || request.ResponseData.Files == null || request.ResponseData.Files.Count == 0)
                {
                    result = $"Failed to retrieve '{parentNames[i]}' part of '{filePath}' file path.";
                    yield break;
                }

                if (request.ResponseData.Files.Count > 1)
                    Debug.LogWarning($"Multiple '{parentNames[i]}' folders been found.");

                parendId = request.ResponseData.Files[0].Id;
            }
        }

        // Searching the file.
        request = new GoogleDriveFiles.ListRequest();
        request.Fields = new List<string> { "files(id, size, modifiedTime)" };
        request.Q = $"'{parendId}' in parents and name = '{fileName}'";

        yield return request.Send();

        if (request.IsError || request.ResponseData.Files == null || request.ResponseData.Files.Count == 0)
        {
            result = $"Failed to retrieve '{filePath}' file.";
            yield break;
        }

        if (request.ResponseData.Files.Count > 1)
            Debug.LogWarning($"Multiple '{filePath}' files been found.");

        var file = request.ResponseData.Files[0];

        result = string.Format("ID: {0} Size: {1:0.00}MB Modified: {2:dd.MM.yyyy HH:MM:ss}",
            file.Id, file.Size * .000001f, file.CreatedTime);

        downloadRequest = GoogleDriveFiles.Export(file.Id, "text/plain");
        downloadRequest.Send().OnDone += SetResult;

        print(file.Content);

        print("hiii");
    }
*/

}

