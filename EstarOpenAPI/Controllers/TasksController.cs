using Application.Interfaces;
using Google.Cloud.Tasks.V2;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;

namespace EstarOpenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : Controller
    {
        private readonly ITasksService _tasksService;
        private readonly CloudTasksClient _client;
        public TasksController(ITasksService tasksService)
        {
            _tasksService = tasksService;
            _client = CloudTasksClient.Create();
        }
        ///// <summary>
        ///// 添加任务
        ///// </summary>
        ///// <param name="payload"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public async Task<IActionResult> CreateTask([FromBody] string payload)
        //{
        //    // Define the queue path
        //    string queuePath = "";//QueuePath(_projectId, _location, _queueName);

        //    // Create a task
        //    var task = new Google.Cloud.Tasks.V2.Task
        //    {
        //        HttpRequest = new Google.Cloud.Tasks.V2.HttpRequest
        //        {
        //            HttpMethod = Google.Cloud.Tasks.V2.HttpMethod.Post,
        //            Url = "https://your-web-api-url.com/task-handler", // Replace with your endpoint
        //            Body = Google.Protobuf.ByteString.CopyFromUtf8(payload)
        //        },
        //        ScheduleTime = Timestamp.FromDateTime(DateTime.UtcNow.AddSeconds(10)) // 任务将在 X 秒后执行
        //    };

        //    // Add the task to the queue
        //    Google.Cloud.Tasks.V2.Task createdTask = await _client.CreateTaskAsync(queuePath, task);
        //    return Ok(new { TaskName = createdTask.Name });
        //}
        ///// <summary>
        ///// 任务队列唯一名，指定任务将被放入的目标队列
        ///// </summary>
        ///// <param name="projectId"></param>
        ///// <param name="location"></param>
        ///// <param name="queueName"></param>
        ///// <returns></returns>
        //private string QueuePath(string projectId, string location, string queueName)
        //{
        //    return $"projects/{projectId}/locations/{location}/queues/{queueName}";
        //}

    }
}
