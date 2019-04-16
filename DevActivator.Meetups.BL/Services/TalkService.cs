using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevActivator.Meetups.BL.Entities;
using DevActivator.Meetups.BL.Extensions;
using DevActivator.Meetups.BL.Interfaces;
using DevActivator.Meetups.BL.Models;

namespace DevActivator.Meetups.BL.Services
{
    public class TalkService : ITalkService
    {
        private readonly ITalkProvider _talkProvider;
        private readonly ISpeakerProvider _speakerProvider;
        private readonly IUnitOfWork _unitOfWork;

        public TalkService(ITalkProvider talkProvider, ISpeakerProvider speakerProvider, IUnitOfWork unitOfWork)
        {
            _talkProvider = talkProvider;
            _speakerProvider = speakerProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AutocompleteRow>> GetAllTalksAsync()
        {
            var talks = await _talkProvider.GetAllTalksAsync().ConfigureAwait(false);
            return talks
                .Select(x => new AutocompleteRow {Id = x.ExportId, Name = x.Title})
                .ToList();
        }

        public async Task<TalkVm> GetTalkAsync(string talkId)
        {
            var talk = await _talkProvider.GetTalkOrDefaultExtendedAsync(talkId).ConfigureAwait(false);
            return talk.ToVm();
        }

        public async Task<TalkVm> AddTalkAsync(TalkVm talk)
        {
            talk.EnsureIsValid();
            var original = await _talkProvider.GetTalkOrDefaultAsync(talk.Id).ConfigureAwait(false);
            if (original != null)
            {
                throw new FormatException($"Данный {nameof(talk.Id)} \"{talk.Id}\" уже занят");
            }

            var speakers = await _speakerProvider.GetSpeakersByIdsAsync(talk.SpeakerIds);
            var entity = new Talk {ExportId = talk.Id, Speakers = new List<SpeakerTalk>()}.Extend(talk);
            foreach (var speaker in speakers)
            {
                entity.Speakers.Add(new SpeakerTalk
                {
                    Speaker = speaker,
                    Talk = entity
                });
            }


            var res = await _talkProvider.SaveTalkAsync(entity).ConfigureAwait(false);
            return res.ToVm();
        }

        public async Task<TalkVm> UpdateTalkAsync(TalkVm talk)
        {
            talk.EnsureIsValid();
            
            var original = await _talkProvider.GetTalkOrDefaultExtendedAsync(talk.Id).ConfigureAwait(false);
            original.ExportId = talk.Id;
            original.Title = talk.Title;
            original.Description = talk.Description;
            original.CodeUrl = talk.CodeUrl;
            original.SlidesUrl = talk.SlidesUrl;
            original.VideoUrl = talk.VideoUrl;
            
            var speakers = await _speakerProvider.GetSpeakersByIdsAsync(talk.SpeakerIds);
            foreach (var speaker in speakers)
            {
                if (original.Speakers.Any(x => x.SpeakerId == speaker.Id))
                    continue;
                    
                original.Speakers.Add(new SpeakerTalk
                {
                    Speaker = speaker,
                    Talk = original
                });
            }

            await _unitOfWork.SaveChangesAsync();
            return original.ToVm();
        }
    }
}