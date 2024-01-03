using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LoadingChecker : Singelton<LoadingChecker>
{
	public delegate void LoadingCallback();
	public LoadingCallback onLoadingFinished;

	List<AsyncOperation> asyncOperations = new List<AsyncOperation>();
	List<Task> tasks = new List<Task>();
	public List<AsyncOperation> AsyncOperations { get { return asyncOperations; } }
	public List<Task> Tasks { get {  return tasks; } }	

	bool finishLoading = true;
	public bool FinishLoading { get { return finishLoading; } }


	public async void StartCheckingLoading()
	{
		finishLoading = false;
		bool loading = true;
		while (loading)
		{
			loading = false;
			foreach (AsyncOperation asyncOperation in AsyncOperations)
			{
				if (asyncOperation == null) { loading = true; continue; }
				if (!asyncOperation.isDone) loading = true;
			}
			await new WaitForSecondsRealtime(0.1f);
		}
		await Task.WhenAll(Tasks);
		finishLoading = true;
		if (onLoadingFinished != null) onLoadingFinished();
		ClearLoadingCache();
	}

	public void ClearLoadingCache()
	{
		tasks.Clear();
		asyncOperations.Clear();
	}
}
