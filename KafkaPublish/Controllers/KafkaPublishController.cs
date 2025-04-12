using Confluent.Kafka;
using KafkaCore;
using KafkaModel;
using Microsoft.AspNetCore.Mvc;

namespace KafkaPublish.API.Controllers
{
    /// <summary>
    /// api thực hiện publish message lên kafka
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class KafkaPublishController : ControllerBase
    {
        #region Declare

        #endregion

        #region Constructor

        /// <summary>
        /// Khởi tạo controller
        /// </summary>
        public KafkaPublishController() { }

        #endregion

        #region API



        #endregion

        #region Methods

        /// <summary>
        /// test việc publish message lên kafka
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Publish")]
        public async Task<IActionResult> Publish([FromBody] KafkaMessage message)
        {
            try
            {
                KafkaPublisher publisher = new KafkaPublisher();

                KafkaConfig config = ConfigUtil.CenterConfig.KafkaPublishConfig;
                await publisher.PublishAsync(config, message.Message, message.Sequency);
                return Ok("Publish success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
